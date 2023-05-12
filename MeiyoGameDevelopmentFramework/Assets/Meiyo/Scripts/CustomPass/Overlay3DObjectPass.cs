using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

class Overlay3DObjectPass : CustomPass
{
    [SerializeField] private string targetName;
    [SerializeField] private string targetMaterial;
    private Transform mainCamera;

    private Vector3 scale = new Vector3(1, 1, 1);
    public Vector3 Scale { get { return scale; } set { scale = value; } }

    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in an performance manner.
    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        // Setup code here
    }

    protected override void Execute(CustomPassContext ctx)
    {
        // Executed every frame for all the camera inside the pass volume.
        // The context contains the command buffer to use to enqueue graphics commands.
        if (ctx.hdCamera.camera.transform.name == "Main Camera")
            mainCamera = ctx.hdCamera.camera.transform;

        // Debug.Log(mainCamera.name + ": " + ", Position: " + mainCamera.position + ", Rotation: " + mainCamera.rotation + ", Scale: " + scale);

        // Set the target to the main view.
        // This tells Unity that we want to render onto the main camera's color buffer. This is the final image that will be displayed on the screen.
        ctx.cmd.SetRenderTarget(ctx.cameraColorBuffer);

        // Set up the object's material properties.
        // This creates a new MaterialPropertyBlock, which is a container for shader properties.
        // We can set various properties on this block and then apply them to the mesh when we render it.
        var materialPropertyBlock = new MaterialPropertyBlock();
        // This sets the base color of the material to red. "_BaseColor" is a standard shader property that most HDRP shaders will recognize.
        materialPropertyBlock.SetColor("_BaseColor", Color.red);

        // Render your object here.
        // You'll need to manually render your object using CommandBuffer functions.
        // The mesh defines the shape of the object, and the material defines how it will look when rendered.
        GameObject go = Resources.Load<GameObject>(targetName); // We load the gameobject because we are not currently storing just the mesh in the resources folder.
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;  // We can get the mesh from the gameobject that we have loaded
        Material material = Resources.Load<Material>(targetMaterial); // The material to apply to the mesh.

        // Define the object's position, rotation (scale is determined by a variable, sine it can be adjusted by scrolling the mouse wheel)
        Vector3 mainCameraPos = mainCamera.position;
        Vector3 position = mainCameraPos;//+ new Vector3(1, 2.2f, 0); // new Vector3(1, 2.2f, 1);
        Quaternion mainCameraRot = mainCamera.rotation;
        Quaternion rotation = mainCameraRot; // Quaternion.identity;

        // This creates a transformation matrix for the object.This matrix will position, rotate, and scale the object in the world.
        Matrix4x4 transform = Matrix4x4.TRS(position, rotation, scale);

        // This issues a command to draw the mesh with the given transformation, material, and material properties.
        // The 0, 0 parameters specify the sub-mesh index and shader pass index, respectively. In most cases, you can just leave these as 0.
        ctx.cmd.DrawMesh(mesh, transform, material, 0, 0, materialPropertyBlock);
    }

    protected override void Cleanup()
    {
        // Cleanup code
    }
}