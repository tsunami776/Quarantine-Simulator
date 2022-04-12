using System;
using System.Collections.Generic;
using System.Linq;
using ImmersiveVRTools.PublisherTools.WelcomeScreen;
using ImmersiveVRTools.PublisherTools.WelcomeScreen.GuiElements;
using ImmersiveVRTools.PublisherTools.WelcomeScreen.PreferenceDefinition;
using ImmersiveVRTools.PublisherTools.WelcomeScreen.Utilities;
using ReliableSolutions.Unity.Common.Editor.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ImmersiveVRInventoryWelcomeScreen : ProductWelcomeScreenBase
{
    public static string BaseUrl = "https://immersivevrtools.com";
    private static readonly string RedirectBaseUrl = "https://immersivevrtools.com/redirect/immersive-vr-inventory";

    public static string GenerateGetUpdatesUrl(string userId, string versionId)
    {
        return $"{BaseUrl}/updates/immersive-vr-inventory/{userId}?CurrentVersion={versionId}";
    }
    public static string VersionId = "1.2.1";
    private static readonly string ProjectIconName = "ProductIcon64";
    public static readonly string ProjectName = "immersive-vr-inventory";

    private static Vector2 _WindowSizePx = new Vector2(650, 500);
    private static string _WindowTitle = "Immersive VR Inventory";

    private static readonly List<GuiSection> LeftSections = new List<GuiSection>() {
        new GuiSection("", new List<ClickableElement>
        {
            new LastUpdateButton("New Update!", (screen) => LastUpdateUpdateScrollViewSection.RenderMainScrollViewSection(screen)),
            new ChangeMainViewButton("Welcome", (screen) => MainScrollViewSection.RenderMainScrollViewSection(screen)),
        }),
        new GuiSection("Options", new List<ClickableElement>
        {
            new ChangeMainViewButton("VR Integrations", (screen) =>
            {
                GUILayout.Label(
                    @"XR Toolkit will require some dependencies to run, please have a look in documentation, it should only take a few moments to set up.",
                    screen.TextStyle
                );

                using (LayoutHelper.LabelWidth(200))
                {
                    ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRInventoryPreference.EnableXrToolkitIntegrationPreferenceDefinition);
                }

                const int sectionBreakHeight = 15;
                GUILayout.Space(sectionBreakHeight);

                GUILayout.Label(
                    @"VRTK require some dependencies to run, please have a look in documentation, it should only take a few moments to set up.",
                    screen.TextStyle
                );

                using (LayoutHelper.LabelWidth(200))
                {
                    ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRInventoryPreference.EnableSteamVRIntegrationPreferenceDefinition);
                }
                GUILayout.Space(sectionBreakHeight);

            }),
            new ChangeMainViewButton("Shaders", (screen) =>
            {
                GUILayout.Label(
                    @"By default package uses HDRP shaders, you can change those to standard surface shaders from dropdown below",
                    screen.TextStyle
                );

                using (LayoutHelper.LabelWidth(200))
                {
                    ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRInventoryPreference.ShaderModePreferenceDefinition);
                }
            })
        }),
        new GuiSection("Launch Demo", new List<ClickableElement>
        {
            new LaunchSceneButton("XR Toolkit", (s) => FindSceneAssetPath("XRToolkitDemoScene")),
            new LaunchSceneButton("Steam VR", (s) => FindSceneAssetPath("SteamVRDemoScene"))
        })
    };

    private static readonly GuiSection TopSection = new GuiSection("Support", new List<ClickableElement>
        {
            new OpenUrlButton("Documentation", $"{RedirectBaseUrl}/documentation"),
            new OpenUrlButton("Unity Forum", $"{RedirectBaseUrl}/unity-forum"),
            new OpenUrlButton("Contact", $"{RedirectBaseUrl}/contact")
        }
    );

    private static readonly GuiSection BottomSection = new GuiSection(
        "Can I ask for a quick favour?",
        $"I'd be great help if you could spend few minutes to leave a review on:",
        new List<ClickableElement>
        {
            new OpenUrlButton("  Unity Asset Store", $"{RedirectBaseUrl}/unity-asset-store"),
        }
    );

    private static readonly ScrollViewGuiSection MainScrollViewSection = new ScrollViewGuiSection(
        "", (screen) =>
        {
            GenerateCommonWelcomeText(ImmersiveVRInventoryPreference.ProductName, screen, "Default demo implementation is with Unity XR Toolkit, you need to get it from Package Manager otherwise you'll see compilation errors. Or if you want to use different implementation just untick Unity XR Toolkit Integration box in 'Quick adjustments' section below.");

            GUILayout.Label("Quick adjustments:", screen.LabelStyle);
            using (LayoutHelper.LabelWidth(220))
            {
                ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRInventoryPreference.EnableXrToolkitIntegrationPreferenceDefinition);
                ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRInventoryPreference.EnableSteamVRIntegrationPreferenceDefinition);
                ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRInventoryPreference.ShaderModePreferenceDefinition);
            }
        }
    );

    private static readonly ScrollViewGuiSection LastUpdateUpdateScrollViewSection = new ScrollViewGuiSection(
        "New Update", (screen) =>
        {
            GUILayout.Label(screen.LastUpdateText, screen.BoldTextStyle, GUILayout.ExpandHeight(true));
        }
    );

    private static string FindSceneAssetPath(string sceneName)
    {
        var scene = AssetDatabase.FindAssets("t:Scene " + sceneName).FirstOrDefault();
        return scene != null ? AssetDatabase.GUIDToAssetPath(scene) : null;
    }

    public override string WindowTitle { get; } = _WindowTitle;
    public override Vector2 WindowSizePx { get; } = _WindowSizePx;


    [MenuItem("Window/Immersive VR Inventory/Start Screen", false, 1999)]
    public static void Init()
    {
        OpenWindow<ImmersiveVRInventoryWelcomeScreen>(_WindowTitle, _WindowSizePx);
    }

    public void OnEnable()
    {
        OnEnableCommon(ProjectIconName);
    }

    public void OnGUI()
    {
        RenderGUI(LeftSections, TopSection, BottomSection, MainScrollViewSection);
    }
}

public class ImmersiveVRInventoryPreference : ProductPreferenceBase
{
    public static string BuildSymbol_EnableXrToolkit = "INTEGRATIONS_XRTOOLKIT";
    public static string BuildSymbol_EnableSteamVR = "INTEGRATIONS_STEAMVR";

    public const string ProductName = "Immersive VR Inventory";
    private static string[] ProductKeywords = new[] { "start", "vr", "tools" };

    public static readonly ToggleProjectEditorPreferenceDefinition EnableXrToolkitIntegrationPreferenceDefinition = new ToggleProjectEditorPreferenceDefinition(
        "Enable Unity XR Toolkit integration", "XRToolkitIntegrationEnabled", true,
        (newValue, oldValue) =>
        {
            BuildDefineSymbolManager.SetBuildDefineSymbolState(BuildSymbol_EnableXrToolkit, (bool)newValue);
        });

    public static readonly ToggleProjectEditorPreferenceDefinition EnableSteamVRIntegrationPreferenceDefinition = new ToggleProjectEditorPreferenceDefinition(
        "Enable SteamVR integration", "SteamVRIntegrationEnabled", false,
        (newValue, oldValue) =>
        {
            BuildDefineSymbolManager.SetBuildDefineSymbolState(BuildSymbol_EnableSteamVR, (bool)newValue);
        });

    public static readonly EnumProjectEditorPreferenceDefinition ShaderModePreferenceDefinition = new EnumProjectEditorPreferenceDefinition("Shaders",
        "ShadersMode",
        ShadersMode.HDRP,
        typeof(ShadersMode),
        (newValue, oldValue) =>
        {
            if (oldValue == null) oldValue = default(ShadersMode);

            var newShaderModeValue = (ShadersMode)newValue;
            var oldShaderModeValue = (ShadersMode)oldValue;

            if (newShaderModeValue != oldShaderModeValue) 
            {
                SetMaterialsShaders(newShaderModeValue);
            }
        }
    );

    public static List<string> MaterialNamesToExcludeFromCommonShaderChange = new List<string>()
    {
        "SlotMaterial"
    };

    public static void SetMaterialsShaders(ShadersMode newShaderModeValue)
    {
        var rootToolFolder = AssetPathResolver.GetAssetFolderPathRelativeToScript(ScriptableObject.CreateInstance(typeof(ImmersiveVRInventoryWelcomeScreen)), 1);
        var assets = AssetDatabase.FindAssets("t:Material", new[] { rootToolFolder });

        try
        {
            Shader shaderToUse = null;
            switch (newShaderModeValue)
            {
                case ShadersMode.HDRP: shaderToUse = Shader.Find("HDRP/Lit"); break;
                case ShadersMode.URP: shaderToUse = Shader.Find("Universal Render Pipeline/Lit"); break;
                case ShadersMode.Surface: shaderToUse = Shader.Find("Standard"); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var guid in assets)
            {
                var material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                if (!MaterialNamesToExcludeFromCommonShaderChange.Contains(material.name))
                {
                    if (material.shader.name != shaderToUse.name)
                    {
                        material.shader = shaderToUse;
                    }
                }
            }

            var slotMaterialAssetGuid = AssetDatabase.FindAssets("t:Material SlotMaterial", new[] { rootToolFolder })[0];
            var slotMaterial = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(slotMaterialAssetGuid));
            var useSurfaceSharerForSlots = newShaderModeValue == ShadersMode.Surface;
            if (useSurfaceSharerForSlots)
            {
                if (slotMaterial.shader == null || slotMaterial.shader.name != "Custom/Slot Shader Legacy")
                {
                    slotMaterial.shader = Shader.Find("Custom/Slot Shader Legacy");
                    slotMaterial.color = Color.red;
                    slotMaterial.SetFloat(HolsterSlot.RingRadiusShaderPropertyName, 0.8f);
                    slotMaterial.SetFloat(HolsterSlot.ThicknessShaderPropertyName, 0.2f);
                }
            }
            else
            {
                if (slotMaterial.shader == null || slotMaterial.shader.name != "Shader Graphs/SlotShader")
                {
                    var slotShader = Shader.Find("Shader Graphs/SlotShader");
                    if(slotShader != null) slotMaterial.shader = slotShader;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Shader does not exist: {ex.Message}");
        }
    }

    public static List<ProjectEditorPreferenceDefinitionBase> PreferenceDefinitions = new List<ProjectEditorPreferenceDefinitionBase>()
    {
        CreateDefaultShowOptionPreferenceDefinition(),
        EnableXrToolkitIntegrationPreferenceDefinition,
        EnableSteamVRIntegrationPreferenceDefinition,
        ShaderModePreferenceDefinition
    };

    private static bool PrefsLoaded = false;
    

#if UNITY_2019_1_OR_NEWER
    [SettingsProvider]
    public static SettingsProvider ImpostorsSettings()
    {
        return GenerateProvider(ProductName, ProductKeywords, PreferencesGUI);
    }

#else
	[PreferenceItem(ProductName)]
#endif
    public static void PreferencesGUI()
    {
        if (!PrefsLoaded)
        {
            LoadDefaults(PreferenceDefinitions);
            PrefsLoaded = true;
        }

        RenderGuiCommon(PreferenceDefinitions);
    }

    public enum ShadersMode
    {
        HDRP,
        URP,
        Surface
    }
}

[InitializeOnLoad]
public class ImmersiveVRInventoryWelcomeScreenInitializer : WelcomeScreenInitializerBase
{
    static ImmersiveVRInventoryWelcomeScreenInitializer()
    {
        var userId = ProductPreferenceBase.CreateDefaultUserIdDefinition(ImmersiveVRInventoryWelcomeScreen.ProjectName).GetEditorPersistedValueOrDefault().ToString();

        HandleUnityStartup(
            ImmersiveVRInventoryWelcomeScreen.Init,
            ImmersiveVRInventoryWelcomeScreen.GenerateGetUpdatesUrl(userId, ImmersiveVRInventoryWelcomeScreen.VersionId), 
            (isFirstRun) =>
        {
            AutoDetectAndSetShaderMode();
        });
    }

    private static void AutoDetectAndSetShaderMode()
    {
        var usedShaderMode = ImmersiveVRInventoryPreference.ShadersMode.Surface;
        var renderPipelineAsset = GraphicsSettings.renderPipelineAsset;
        if (renderPipelineAsset == null)
        {
            usedShaderMode = ImmersiveVRInventoryPreference.ShadersMode.Surface;
        }
        else if (renderPipelineAsset.GetType().Name.Contains("HDRenderPipelineAsset"))
        {
            usedShaderMode = ImmersiveVRInventoryPreference.ShadersMode.HDRP;
        }
        else if (renderPipelineAsset.GetType().Name.Contains("UniversalRenderPipelineAsset"))
        {
            usedShaderMode = ImmersiveVRInventoryPreference.ShadersMode.URP;
        }

        ImmersiveVRInventoryPreference.ShaderModePreferenceDefinition.SetEditorPersistedValue(usedShaderMode);
        ImmersiveVRInventoryPreference.SetMaterialsShaders(usedShaderMode);
    }
}