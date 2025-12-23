using Newtonsoft.Json;
using RunVision.Models;
using RunVision.Services;
using RunVision.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VM.Core;
namespace RunVision.Services
{
    public class ProjectConfigService : IAppConfigService
    {
        // 项目根目录
        private readonly string _projectsRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Projects");
        // 当前配置文件路径
        private string _currentFilePath;

        // 当前配置内容
        public ProjectModel CurrentSettings { get; private set; }

        // 方案列表
        public List<string> ProjectNames { get; private set; } = new List<string>();


        public ProjectConfigService()
        {
            if (!Directory.Exists(_projectsRoot))
                Directory.CreateDirectory(_projectsRoot);

            // 扫描有效项目（包含 *.sol 文件）
            foreach (var dir in Directory.GetDirectories(_projectsRoot))
            {
                if (!Directory.GetFiles(dir, "*.sol", SearchOption.TopDirectoryOnly).Any())
                    continue;

                var projectName = Path.GetFileName(dir);
                ProjectNames.Add(projectName);

                var configFile = Path.Combine(dir, "config.json");
                if (!File.Exists(configFile))
                {
                    var defaultConfig = CreateDefaultSettings();
                    File.WriteAllText(configFile, JsonConvert.SerializeObject(defaultConfig, Formatting.Indented));
                }
            }
        }

        /// <summary>
        /// 加载项目，同时加载 config.json 和方案文件 (.sol)
        /// </summary>
        public void LoadProject(string projectName)
        {
            var dir = Path.Combine(_projectsRoot, projectName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _currentFilePath = Path.Combine(dir, "config.json");

            if (!File.Exists(_currentFilePath))
            {
                CurrentSettings = CreateDefaultSettings();
                Save();
            }
            else
            {
                var json = File.ReadAllText(_currentFilePath);
                CurrentSettings = JsonConvert.DeserializeObject<ProjectModel>(json) ?? CreateDefaultSettings();
            }

            VmSolution.Instance.CloseSolution();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var solFile = Directory.GetFiles(dir, "*.sol", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (solFile != null)
            {
                MyLogger.Warn($"开始加载方案...");
                VmSolution.Load(solFile);
                MyLogger.Info($"方案路径：[{solFile}]");
            }

            if (!ProjectNames.Contains(projectName))
                ProjectNames.Add(projectName);
        }

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(_currentFilePath))
            {
                MyLogger.Error("当前没有加载方案，无法保存");
            }
            var json = JsonConvert.SerializeObject(CurrentSettings, Formatting.Indented);
            File.WriteAllText(_currentFilePath, json);
        }

        private ProjectModel CreateDefaultSettings()
        {
            return new ProjectModel
            {
                CamerasConfig = new List<CameraModel>(),
                PlcConfig = new PlcModel(),
                DatabaseConfig = new DatabaseModel(),
                SolutionConfig = new SolutionModel(),
                ImageSaveConfig = new ImageSaveModel()
            };
        }
    }
}