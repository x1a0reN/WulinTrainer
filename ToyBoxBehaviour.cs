using BepInEx;
using HaxxToyBox.Config;
using HaxxToyBox.GUI;
using HaxxToyBox.Utilities;
using System.Reflection;
using WuLin;
using InputManager = UniverseLib.Input.InputManager;
using HaxxToyBox.Config;
using ChartAndGraph;
using System.IO;
using System.Text;
using GameData;
using static WuLin.GameFrameworks.GTTools;
using Object = UnityEngine.Object;

namespace HaxxToyBox;

[RegisterInIl2Cpp]
internal class ToyBoxBehaviour : MonoBehaviour
{
    public static class Constants
    {
        public const string BundleRes = "Assets.toybox";
        public const string BundlePath = "/HaxxToyBox/AssetBundle/toybox";
        public const string AssetName = "ToyBoxCanvas";
    }

    private bool _initialized = false;

    private List<GameObject> Panels = new();

    public Transform buttonBot;

    public Transform buttonBotDLC;

    public GameObject rolePanel;

    public GameObject dlcPanel;

    public PopupPanel Popup;

    public static UIWorldMapManager myworld;

    public static bool WorldMap = false;

    public static bool WorldMap1 = false;

    public static bool WorldMap2 = false;

    public static bool WorldMap3 = false;

    public static bool WorldMap4 = false;

    public static bool WorldMap5 = false;

    public static bool UIConsole = false;

    public static bool isNewVersion = false;

    public static GameCharacterInstance MainPlayer;
    public static ToyBoxBehaviour Instance { get; private set; }
    public GameObject GUICanvas { get; private set; }

    public ToyBoxBehaviour(IntPtr ptr) : base(ptr) { }
    
    public static void Setup()
    {
        Instance = new GameObject("ToyBoxBehaviour").AddComponent<ToyBoxBehaviour>();
        DontDestroyOnLoad(Instance.gameObject);
        Instance.gameObject.hideFlags |= HideFlags.HideAndDontSave;
    }
    
    private void Awake()
    {
        LoadAsset();
        // StartCoroutine(Init());
        SetupUI();

        SetupKongfuPanel();
    }

    //private IEnumerator Init()
    //{
    //    LoadAsset();
    //}
    private void OnEnable()
    {
        SetBlock(true);
    }

    private void OnDisable()
    {
        SetBlock(false);
    }

    private void SetBlock(bool block)
    {
        if (GameTimer.HasInstance) {
            if (block)
                GameTimer.Instance.AddOrSetTimeScale(this, 0);
            else
                GameTimer.Instance.RemoveTimeScale(this);
        }
        if (InGameTimeManager.HasInstance) {
            if (block)
                InGameTimeManager.RegisterTimeBlocker("HaxxToyBox");
            else
                InGameTimeManager.UnRegisterTimeBlocker("HaxxToyBox");
        }
    }

    private void LoadAsset(bool fromFile=false)
    {
        if (fromFile)
            LoadAssetFromFile();
        else
            LoadAssetFromEmbedded();
    }

    private void LoadAssetFromFile()
    {
        if (!File.Exists(Paths.PluginPath + Constants.BundlePath)) {
            ToyBox.LogWarning("Skipping AssetBundle Loading - AssetBundle Doesn't Exist at: " + Paths.PluginPath + Constants.BundlePath);
            return;
        }

        ToyBox.LogMessage($"Trying to load {Constants.AssetName} from {Constants.BundlePath} ...");
        var guiAsset = AssetBundle.LoadFromFile(Paths.PluginPath + Constants.BundlePath);
        if (guiAsset == null) {
            ToyBox.LogMessage("AssetBundle Failed to Load!");
            return;
        }

        ToyBox.LogMessage("Trying to Load Prefab...");
        var guiPrefab = guiAsset.LoadAsset<GameObject>(Constants.AssetName);
        if (guiPrefab != null) {
            ToyBox.LogMessage("Asset Loaded! Trying to Instantiate Prefab...");
            GUICanvas = Instantiate(guiPrefab);
            GUICanvas.name = "ToyBoxCanvas";
            DontDestroyOnLoad(GUICanvas);
            Popup = GUICanvas.AddComponent<PopupPanel>();
            GUICanvas.SetActive(false);

            //trainerGUI = GUICanvas.AddComponent<TrainerGUI>();

            _initialized = true;
        }
        else {
            ToyBox.LogMessage("Failed to Load Asset!");
        }
    }

    private void LoadAssetFromEmbedded()
    {
        ToyBox.LogMessage($"Trying to load {Constants.AssetName} from embedded resources...");

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.{Constants.BundleRes}";

        using (Stream stream = assembly.GetManifestResourceStream(resourceName)) {
            if (stream == null) {
                ToyBox.LogWarning($"Asset bundle not found in embedded resources: {resourceName}");
                return;
            }

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            var guiAsset = AssetBundle.LoadFromMemory(buffer);
            if (guiAsset == null) {
                ToyBox.LogMessage("AssetBundle Failed to Load from Memory!");
                return;
            }

            ToyBox.LogMessage("Trying to Load Prefab...");
            var guiPrefab = guiAsset.LoadAsset<GameObject>(Constants.AssetName);
            if (guiPrefab != null) {
                ToyBox.LogMessage("Asset Loaded! Trying to Instantiate Prefab...");
                ToyBox.LogMessage("LoadAssetFromEmbedded...");
                GUICanvas = Instantiate(guiPrefab);
                GUICanvas.name = "ToyBoxCanvas";
                DontDestroyOnLoad(GUICanvas);
                //Popup = GUICanvas.AddComponent<PopupPanel>();
                GUICanvas.SetActive(false);

                //trainerGUI = GUICanvas.AddComponent<TrainerGUI>();

                _initialized = true;
            }
            else {
                ToyBox.LogMessage("Failed to Load Asset!");
            }
        }
    }

    private void SetupUI()
    {
        buttonBot = GUICanvas.transform.Find("Buttons Bottom");
        foreach (var child in buttonBot) {
            var button = child.Cast<Transform>().Find("Button").GetComponent<Button>();
            button.gameObject.AddComponent<FadeButtonWrapper>();

            var panelName = $"{child.Cast<Transform>().name}Panel";
            var panel = GUICanvas.transform.Find(panelName)?.gameObject;
            if (panel != null) {
                Panels.Add(panel);
                button.onClick.AddListener(() => {
                    for (int i = 0; i < Panels.Count; i++) {
                        Panels[i].SetActive(false);
                    }
                    panel.SetActive(true);
                });
            }
        }

        buttonBotDLC = GUICanvas.transform.Find("Buttons Bottom DLC");
        foreach (var child in buttonBotDLC)
        {
            var button = child.Cast<Transform>().Find("Button").GetComponent<Button>();
            button.gameObject.AddComponent<FadeButtonWrapper>();

            var panelName = $"{child.Cast<Transform>().name}Panel";
            var panel = GUICanvas.transform.Find(panelName)?.gameObject;
            if (panel != null)
            {
                Panels.Add(panel);
                button.onClick.AddListener(() => {
                    for (int i = 0; i < Panels.Count; i++)
                    {
                        Panels[i].SetActive(false);
                    }
                    panel.SetActive(true);
                });
            }
        }

        rolePanel = GUICanvas.transform.Find("RolePanel").gameObject;
        ToyBox.LogMessage(rolePanel.name);
        rolePanel.AddComponent<RolePanel>();

        var traitPanel = GUICanvas.transform.Find("TraitPanel").gameObject;
        traitPanel.AddComponent<TraitPanel>();

        var addTraitButton = rolePanel.transform.Find("AddTrait").GetComponent<Button>();
        addTraitButton.gameObject.AddComponent<FadeButtonWrapper>();
        addTraitButton.onClick.AddListener(() =>
        {
            traitPanel.GetComponent<TraitPanel>().Show();
        });

        var buffPanel = GUICanvas.transform.Find("BuffPanel").gameObject;
        buffPanel.AddComponent<BuffPanel>();

        var addBuffButton = rolePanel.transform.Find("AddBuff").GetComponent<Button>();
        addBuffButton.gameObject.AddComponent<FadeButtonWrapper>();
        addBuffButton.onClick.AddListener(() =>
        {
            buffPanel.GetComponent<BuffPanel>().Show();
        });

        var itemPanel = GUICanvas.transform.Find("ItemPanel").gameObject;
        itemPanel.AddComponent<ItemPanel>();

        var martialPanel = GUICanvas.transform.Find("MartialPanel").gameObject;
        martialPanel.AddComponent<MartialPanel>();

        var miscPanel = GUICanvas.transform.Find("MiscPanel").gameObject;
        miscPanel.AddComponent<MiscPanel>();

        var npcPanel = GUICanvas.transform.Find("NpcPanel").gameObject;
        npcPanel.AddComponent<NpcPanel>();

        dlcPanel = GUICanvas.transform.Find("DlcPanel").gameObject;
        dlcPanel.AddComponent<DlcPanel>();

        var dlctraitPanel = GUICanvas.transform.Find("DlcTraitPanel").gameObject;
        dlctraitPanel.AddComponent<DlcBuffPanel>();

        var dlcaddTraitButton = dlcPanel.transform.Find("AddTrait").GetComponent<Button>();
        dlcaddTraitButton.gameObject.AddComponent<FadeButtonWrapper>();
        dlcaddTraitButton.onClick.AddListener(() =>
        {
            dlctraitPanel.GetComponent<DlcBuffPanel>().Show();
        });

        var dlcitemPanel = GUICanvas.transform.Find("DlcItemPanel").gameObject;
        dlcitemPanel.AddComponent<DlcItemPanel>();

        var dlcmiscPanel = GUICanvas.transform.Find("DlcMiscPanel").gameObject;
        dlcmiscPanel.AddComponent<DlcMiscPanel>();

        var aboutPanel = GUICanvas.transform.Find("AboutPanel").gameObject;
        var tips = GUICanvas.transform.Find("AboutPanel/Top/Tips").gameObject;
        aboutPanel.AddComponent<AboutPanel>();


        #region 检测版本

        string sourceDir = System.Environment.CurrentDirectory + "\\version.txt";
        if (!File.Exists(sourceDir))
        {
            FileStream fs_write = new FileStream(sourceDir, FileMode.Create);
            byte[] data = System.Text.Encoding.Default.GetBytes("3.7.5");
            fs_write.Write(data, 0, data.Length);
            fs_write.Flush();
            fs_write.Close();
            #region 版本更新
            
            isNewVersion = true;
            rolePanel.SetActive(false);
            aboutPanel.SetActive(true);
            tips.SetActive(true);

            #endregion
        }
        else {
            StreamReader sr = new StreamReader(sourceDir, Encoding.Default);
            String line = sr.ReadLine();
            if (line != null) {
                ToyBox.LogMessage("version:" + line);
                if (line.ToString() != "3.7.5")
                {
                    sr.Close();
                    FileStream fs = new FileStream(sourceDir, FileMode.Create);
                    //获得字节数组
                    byte[] data = System.Text.Encoding.Default.GetBytes("3.7.5");
                    //开始写入
                    fs.Write(data, 0, data.Length);
                    //清空缓冲区、关闭流
                    fs.Flush();
                    fs.Close();

                    #region 版本更新

                    isNewVersion = true;
                    rolePanel.SetActive(false);
                    aboutPanel.SetActive(true);
                    tips.SetActive(true);

                    #endregion

                }
            }

        }
        #endregion

    }

    private void SetupKongfuPanel()
    {
        // 拿到最外层的武学界面
        var MartialArtsPanel = UiSingletonPrefab<UIMenuPanel>.Instance.panel[2].transform;
        // 拿到功夫界面
        var KongfuPanel = MartialArtsPanel.Find("MartialArts/KongFu/KongFu");
        // 把卸下功夫界面的滚轮组件拷贝到功夫界面
        var Scrollcomp = Object.Instantiate<Transform>(MartialArtsPanel.Find("MartialArts/UniqueSkill/ScrollView"), KongfuPanel.parent, false).GetComponent<ScrollRect>();
        KongfuPanel.SetParent(Scrollcomp.content.parent);
        Object.DestroyImmediate(Scrollcomp.content.gameObject);
        // 修改滚轮组件属性
        Scrollcomp.content = KongfuPanel.GetComponent<RectTransform>();
        Scrollcomp.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        Scrollcomp.GetComponent<RectTransform>().sizeDelta = new Vector2(865.7728f, 540.16f);
        Scrollcomp.transform.localPosition = new Vector3(0f, -275f, 0f);
    }

    public void ShowCanvas()
    {
        GUICanvas.SetActive(true);
        //Popup.Open();
    }

    public void HideCanvas()
    {
        GUICanvas.SetActive(false);
        //Popup.Close();
    }

    public void Update()
    {
        if (InputManager.GetKey(KeyCode.LeftControl) && InputManager.GetKeyDown(KeyCode.F8))
        {
            var uiconsolePanel = UiSingletonPrefab<ConsoleUI>.Instance;
            UIConsole = !UIConsole;
            if (UIConsole) {
                uiconsolePanel.Show(true);
            }
            else
            {
                uiconsolePanel.Hide(true);
            }
            
        }
        if (_initialized && InputManager.GetKeyDown(ConfigManager.Canvas_Toggle.Value) && MonoSingleton<PlayerTeamManager>.Instance != null 
            && PlayerCreateManager.Instance != null && RoamingManager.Instance != null && Player.Instance != null
            || (Dlc1SlgManager.Instance.factionInAction != null && InputManager.GetKeyDown(ConfigManager.Canvas_Toggle.Value)))
        {
            if (GUICanvas.active)
                HideCanvas();
            else
                ShowCanvas();
        }

        if (InputManager.GetKeyDown(ConfigManager.Recover_Toggle.Value)) {
            MiscPanel.RecoverAll();
        }

        if (InputManager.GetKeyDown(ConfigManager.SpeedUp_Toggle.Value)) {
            MiscPanel.SpeedUp();
        }

        if (InputManager.GetKeyDown(ConfigManager.SpeedDown_Toggle.Value)) {
            MiscPanel.SpeedDown();
        }
        if (InputManager.GetKeyDown(ConfigManager.Map0.Value) && MonoSingleton<PlayerTeamManager>.Instance != null && PlayerCreateManager.Instance != null && RoamingManager.Instance != null && Player.Instance != null)
        {
            UIWorldMapManager.Instance.SetCurMap(1000);
            ToyBoxBehaviour.myworld.curMapUI = UIWorldMapManager.Instance.curMapUI;

            WorldMap = !WorldMap;
            if (WorldMap)
            {
                //myworld.curMapUI.imgHead.gameObject.SetActive(false);
                //myworld.curMapUI.BtnEnter.gameObject.SetActive(false);
                myworld.Show(false);
            }
            else
            {
                //myworld.curMapUI.imgHead.gameObject.SetActive(true);
                //myworld.curMapUI.BtnEnter.gameObject.SetActive(true);
                myworld.Hide(false);

            }
        }
        if (InputManager.GetKeyDown(ConfigManager.Map1.Value) && MonoSingleton<PlayerTeamManager>.Instance != null && PlayerCreateManager.Instance != null && RoamingManager.Instance != null && Player.Instance != null)
        {
            UIWorldMapManager.Instance.SetCurMap(1001);
            ToyBoxBehaviour.myworld.curMapUI = UIWorldMapManager.Instance.curMapUI;

            WorldMap1 = !WorldMap1;
            if (WorldMap1)
            {
                //myworld.curMapUI.imgHead.gameObject.SetActive(false);
                //myworld.curMapUI.BtnEnter.gameObject.SetActive(false);
                myworld.Show(false);
            }
            else
            {
                //myworld.curMapUI.imgHead.gameObject.SetActive(true);
                //myworld.curMapUI.BtnEnter.gameObject.SetActive(true);
                myworld.Hide(false);

            }
        }
        if (InputManager.GetKeyDown(ConfigManager.Map2.Value) && MonoSingleton<PlayerTeamManager>.Instance != null && PlayerCreateManager.Instance != null && RoamingManager.Instance != null && Player.Instance != null)
        {
            UIWorldMapManager.Instance.SetCurMap(1002);
            ToyBoxBehaviour.myworld.curMapUI = UIWorldMapManager.Instance.curMapUI;

            WorldMap2 = !WorldMap2;
            if (WorldMap2)
            {
                //myworld.curMapUI.imgHead.gameObject.SetActive(false);
                //myworld.curMapUI.BtnEnter.gameObject.SetActive(false);
                myworld.Show(false);
            }
            else
            {
                //myworld.curMapUI.imgHead.gameObject.SetActive(true);
                //myworld.curMapUI.BtnEnter.gameObject.SetActive(true);
                myworld.Hide(false);

            }
        }
        if (InputManager.GetKeyDown(ConfigManager.Map3.Value) && MonoSingleton<PlayerTeamManager>.Instance != null && PlayerCreateManager.Instance != null && RoamingManager.Instance != null && Player.Instance != null)
        {
            UIWorldMapManager.Instance.SetCurMap(1003);
            ToyBoxBehaviour.myworld.curMapUI = UIWorldMapManager.Instance.curMapUI;

            WorldMap3 = !WorldMap3;
            if (WorldMap3)
            {
                //myworld.curMapUI.imgHead.gameObject.SetActive(false);
                //myworld.curMapUI.BtnEnter.gameObject.SetActive(false);
                myworld.Show(false);
            }
            else
            {
                //myworld.curMapUI.imgHead.gameObject.SetActive(true);
                //myworld.curMapUI.BtnEnter.gameObject.SetActive(true);
                myworld.Hide(false);

            }
        }
        if (InputManager.GetKeyDown(ConfigManager.Map4.Value) && MonoSingleton<PlayerTeamManager>.Instance != null && PlayerCreateManager.Instance != null && RoamingManager.Instance != null && Player.Instance != null)
        {
            UIWorldMapManager.Instance.SetCurMap(1004);
            ToyBoxBehaviour.myworld.curMapUI = UIWorldMapManager.Instance.curMapUI;

            WorldMap4 = !WorldMap4;
            if (WorldMap4)
            {
                //myworld.curMapUI.imgHead.gameObject.SetActive(false);
                //myworld.curMapUI.BtnEnter.gameObject.SetActive(false);
                myworld.Show(false);
            }
            else
            {
                //myworld.curMapUI.imgHead.gameObject.SetActive(true);
                //myworld.curMapUI.BtnEnter.gameObject.SetActive(true);
                myworld.Hide(false);

            }
        }
        if (InputManager.GetKeyDown(ConfigManager.Map5.Value) && MonoSingleton<PlayerTeamManager>.Instance != null && PlayerCreateManager.Instance != null && RoamingManager.Instance != null && Player.Instance != null)
        {
            UIWorldMapManager.Instance.SetCurMap(1005);
            ToyBoxBehaviour.myworld.curMapUI = UIWorldMapManager.Instance.curMapUI;

            WorldMap5 = !WorldMap5;
            if (WorldMap5)
            {
                //myworld.curMapUI.imgHead.gameObject.SetActive(false);
                //myworld.curMapUI.BtnEnter.gameObject.SetActive(false);
                myworld.Show(false);
            }
            else
            {
                //myworld.curMapUI.imgHead.gameObject.SetActive(true);
                //myworld.curMapUI.BtnEnter.gameObject.SetActive(true);
                myworld.Hide(false);

            }
        }
        if (Input.GetKeyDown(ConfigManager.RefreshBounty_Toggle.Value) && MonoSingleton<PlayerTeamManager>.Instance != null && PlayerCreateManager.Instance != null && RoamingManager.Instance != null && Player.Instance != null)
        {
            SaveObjectBountyHunter.FactionTaskBoardData taskBoardData = BountyHunterManager.Instance.dataContainer.initedObject.GetFactionTaskBoard(BountyHunterUI.Instance.BountyHunterFactionID);
            foreach (SaveObjectBountyHunter.TaskData taskData in taskBoardData.listTaskData)
            {
                if (taskData.taskState != WuLin.TaskState.Open && taskData.taskState != WuLin.TaskState.Accept)
                {
                    taskData.taskState = WuLin.TaskState.Open;
                }
            }
            ConfigManager.isHook = false;
            BountyHunterManager.Instance.TryCreateTask(BountyHunterUI.Instance.BountyHunterFactionID, BountyHunterUI.Instance.factionData);
            ConfigManager.isHook = true;

            BountyHunterUI.Instance.UpdateTaskUI();
            BountyHunterUI.Instance.UpdateLessQuarter();
        }




        //if (InputManager.GetKeyDown(KeyCode.F2)) {
        //    MartialPanel.WriteMartialToCSV("MartialData.csv");
        //}
    }

}
