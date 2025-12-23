using RunVision.Models;
using System.Collections.Generic;

namespace RunVision.Services
{
    public interface IAppConfigService
    {
        // 方案列表
        List<string> ProjectNames { get; }
        //配置内容
        ProjectModel CurrentSettings { get; }
        //加载项目
        void LoadProject(string projectName);
        //保存配置
        void Save();
    }

}
