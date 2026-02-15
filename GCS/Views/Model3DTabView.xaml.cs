using HelixToolkit.Wpf;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace GCS.Views;

public partial class Model3DTabView : UserControl
{
    // Configuration
    private const string DefaultStlPath = "C:\\Users\\samad.samadov\\Desktop\\GCS\\GCS\\Models\\WCR.master_1.stl";
    private const double ModelScale = 0.001;
    private const double InitialYawOffset = 75.0;
    private const int UpdateIntervalMs = 33;

    // 3D Transform State
    private Transform3DGroup? _modelTransformGroup;
    private AxisAngleRotation3D? _rotationRoll;
    private AxisAngleRotation3D? _rotationPitch;
    private AxisAngleRotation3D? _rotationYaw;
    private bool _modelLoaded;

    // Update Throttling
    private readonly DispatcherTimer _updateTimer;
    private double _targetRoll;
    private double _targetPitch;
    private double _targetYaw;
    private bool _needsUpdate;

    // ═══════════════════════════════════════════════════════════════
    // Dependency Properties
    // ═══════════════════════════════════════════════════════════════

    public static readonly DependencyProperty RollProperty =
        DependencyProperty.Register(nameof(Roll), typeof(double), typeof(Model3DTabView),
            new PropertyMetadata(0.0, OnAttitudeChanged));

    public static readonly DependencyProperty PitchProperty =
        DependencyProperty.Register(nameof(Pitch), typeof(double), typeof(Model3DTabView),
            new PropertyMetadata(0.0, OnAttitudeChanged));

    public static readonly DependencyProperty YawProperty =
        DependencyProperty.Register(nameof(Yaw), typeof(double), typeof(Model3DTabView),
            new PropertyMetadata(0.0, OnAttitudeChanged));

    public static readonly DependencyProperty StlModelPathProperty =
        DependencyProperty.Register(nameof(StlModelPath), typeof(string), typeof(Model3DTabView),
            new PropertyMetadata(DefaultStlPath, OnStlPathChanged));

    public double Roll
    {
        get => (double)GetValue(RollProperty);
        set => SetValue(RollProperty, value);
    }

    public double Pitch
    {
        get => (double)GetValue(PitchProperty);
        set => SetValue(PitchProperty, value);
    }

    public double Yaw
    {
        get => (double)GetValue(YawProperty);
        set => SetValue(YawProperty, value);
    }

    public string StlModelPath
    {
        get => (string)GetValue(StlModelPathProperty);
        set => SetValue(StlModelPathProperty, value);
    }

    private static void OnAttitudeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Model3DTabView view)
        {
            view._targetRoll = view.Roll;
            view._targetPitch = view.Pitch;
            view._targetYaw = view.Yaw;
            view._needsUpdate = true;
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // Constructor
    // ═══════════════════════════════════════════════════════════════

    public Model3DTabView()
    {
        InitializeComponent();

        _updateTimer = new DispatcherTimer(DispatcherPriority.Render)
        {
            Interval = TimeSpan.FromMilliseconds(UpdateIntervalMs)
        };
        _updateTimer.Tick += OnUpdateTimerTick;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        IsVisibleChanged += OnVisibleChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadSTLModel();
        if (IsVisible) _updateTimer.Start();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _updateTimer.Stop();
    }

    private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is true && IsLoaded) _updateTimer.Start();
        else _updateTimer.Stop();
    }

    private void OnUpdateTimerTick(object? sender, EventArgs e)
    {
        if (!_needsUpdate || !_modelLoaded) return;
        _needsUpdate = false;
        UpdateModelRotation();
    }

    private static void OnStlPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Model3DTabView view) view.LoadSTLModel();
    }

    private void LoadSTLModel()
    {
        try
        {
            string stlPath = ResolveStlPath();
            if (!File.Exists(stlPath))
            {
                LoadFallbackModel();
                return;
            }

            var importer = new StLReader();
            var model = importer.Read(stlPath);
            if (model == null)
            {
                LoadFallbackModel();
                return;
            }

            // Calculate model bounds and center
            var bounds = model.Bounds;
            var center = new Point3D(
                (bounds.X + bounds.SizeX / 2),
                (bounds.Y + bounds.SizeY / 2),
                (bounds.Z + bounds.SizeZ / 2));

            SetupModelTransforms(model, center);
            ApplyMaterial(model);
            UAVModelVisual.Content = model;
            _modelLoaded = true;
        }
        catch
        {
            LoadFallbackModel();
        }
    }

    private string ResolveStlPath()
    {
        string path = StlModelPath ?? DefaultStlPath;
        if (!Path.IsPathRooted(path))
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        return path;
    }

    private void SetupModelTransforms(Model3DGroup model, Point3D modelCenter)
    {
        _modelTransformGroup = new Transform3DGroup();

        // 1. First, translate the model so its center is at the origin
        //    This ensures rotation happens around the model's center
        _modelTransformGroup.Children.Add(new TranslateTransform3D(
            -modelCenter.X,
            -modelCenter.Y,
            -modelCenter.Z));

        // 2. Apply scale
        _modelTransformGroup.Children.Add(new ScaleTransform3D(ModelScale, ModelScale, ModelScale));

        // 3. Apply initial orientation offset
        _modelTransformGroup.Children.Add(new RotateTransform3D(
            new AxisAngleRotation3D(new Vector3D(0, 0, 1), InitialYawOffset)));

        // 4. Dynamic rotations (around origin, which is now the model center)
        _rotationYaw = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
        _rotationPitch = new AxisAngleRotation3D(new Vector3D(-1, 0, 0), 0);
        _rotationRoll = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);

        _modelTransformGroup.Children.Add(new RotateTransform3D(_rotationYaw));
        _modelTransformGroup.Children.Add(new RotateTransform3D(_rotationPitch));
        _modelTransformGroup.Children.Add(new RotateTransform3D(_rotationRoll));

        model.Transform = _modelTransformGroup;
    }

    private void ApplyMaterial(Model3DGroup model)
    {
        var material = new DiffuseMaterial(new SolidColorBrush(Colors.AntiqueWhite));
        material.Freeze();
        foreach (var child in model.Children)
        {
            if (child is GeometryModel3D geometry)
            {
                geometry.Material = material;
                geometry.BackMaterial = material;
            }
        }
    }

    private void LoadFallbackModel()
    {
        var model = new Model3DGroup();

        var bodyMaterial = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(80, 80, 90)));
        bodyMaterial.Freeze();
        var noseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.OrangeRed));
        noseMaterial.Freeze();

        // Fuselage (already centered at origin)
        var fuselage = new GeometryModel3D(CreateBoxMesh(0.4, 0.1, 0.08), bodyMaterial);
        fuselage.BackMaterial = bodyMaterial;
        model.Children.Add(fuselage);

        // Wing
        var wing = new GeometryModel3D(CreateBoxMesh(0.08, 0.6, 0.02), bodyMaterial);
        wing.BackMaterial = bodyMaterial;
        model.Children.Add(wing);

        // Tail vertical
        var tailVert = new GeometryModel3D(CreateBoxMesh(0.02, 0.02, 0.1), bodyMaterial);
        tailVert.BackMaterial = bodyMaterial;
        tailVert.Transform = new TranslateTransform3D(-0.18, 0, 0.05);
        model.Children.Add(tailVert);

        // Tail horizontal
        var tailHoriz = new GeometryModel3D(CreateBoxMesh(0.02, 0.2, 0.02), bodyMaterial);
        tailHoriz.BackMaterial = bodyMaterial;
        tailHoriz.Transform = new TranslateTransform3D(-0.18, 0, 0.08);
        model.Children.Add(tailHoriz);

        // Nose
        var nose = new GeometryModel3D(CreatePyramidMesh(0.1, 0.05), noseMaterial);
        nose.BackMaterial = noseMaterial;
        nose.Transform = new TranslateTransform3D(0.2, 0, 0);
        model.Children.Add(nose);

        // Fallback model is already centered, pass zero center
        SetupModelTransforms(model, new Point3D(0, 0, 0));
        UAVModelVisual.Content = model;
        _modelLoaded = true;
    }

    private static MeshGeometry3D CreateBoxMesh(double sizeX, double sizeY, double sizeZ)
    {
        var mesh = new MeshGeometry3D();
        double hx = sizeX / 2, hy = sizeY / 2, hz = sizeZ / 2;

        mesh.Positions.Add(new Point3D(-hx, -hy, -hz));
        mesh.Positions.Add(new Point3D(hx, -hy, -hz));
        mesh.Positions.Add(new Point3D(hx, hy, -hz));
        mesh.Positions.Add(new Point3D(-hx, hy, -hz));
        mesh.Positions.Add(new Point3D(-hx, -hy, hz));
        mesh.Positions.Add(new Point3D(hx, -hy, hz));
        mesh.Positions.Add(new Point3D(hx, hy, hz));
        mesh.Positions.Add(new Point3D(-hx, hy, hz));

        int[] indices = { 0, 1, 2, 0, 2, 3, 4, 6, 5, 4, 7, 6, 3, 2, 6, 3, 6, 7, 0, 5, 1, 0, 4, 5, 0, 3, 7, 0, 7, 4, 1, 5, 6, 1, 6, 2 };
        foreach (var i in indices) mesh.TriangleIndices.Add(i);

        mesh.Freeze();
        return mesh;
    }

    private static MeshGeometry3D CreatePyramidMesh(double length, double baseRadius)
    {
        var mesh = new MeshGeometry3D();
        double hl = length / 2;

        mesh.Positions.Add(new Point3D(hl, 0, 0));
        mesh.Positions.Add(new Point3D(-hl, -baseRadius, -baseRadius));
        mesh.Positions.Add(new Point3D(-hl, baseRadius, -baseRadius));
        mesh.Positions.Add(new Point3D(-hl, baseRadius, baseRadius));
        mesh.Positions.Add(new Point3D(-hl, -baseRadius, baseRadius));

        int[] indices = { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 1, 1, 3, 2, 1, 4, 3 };
        foreach (var i in indices) mesh.TriangleIndices.Add(i);

        mesh.Freeze();
        return mesh;
    }

    private void UpdateModelRotation()
    {
        if (_rotationRoll == null || _rotationPitch == null || _rotationYaw == null) return;
        _rotationRoll.Angle = _targetRoll;
        _rotationPitch.Angle = -_targetPitch;
        _rotationYaw.Angle = _targetYaw;
    }

    public void ReloadModel()
    {
        _modelLoaded = false;
        LoadSTLModel();
    }

    public void ResetCamera()
    {
        Viewport3D.Camera.Position = new Point3D(2, 2, 2);
        Viewport3D.Camera.LookDirection = new Vector3D(-1, -1, -1);
        Viewport3D.Camera.UpDirection = new Vector3D(0, 0, 1);
    }
}