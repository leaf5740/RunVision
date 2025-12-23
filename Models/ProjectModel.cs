using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunVision.Models
{
    public class ProjectModel
    {
        //相机配置列表
        public List<CameraModel> CamerasConfig { get; set; } = new List<CameraModel>();
        //PLC配置
        public PlcModel PlcConfig { get; set; } = new PlcModel();
        //数据库配置
        public DatabaseModel DatabaseConfig { get; set; } = new DatabaseModel();
        //方案配置
        public SolutionModel SolutionConfig { get; set; } = new SolutionModel();
        //图像保存配置
        public ImageSaveModel ImageSaveConfig { get; set; } = new ImageSaveModel();
    }

    public class CameraModel
    {
        //相机品牌
        public string Brand { get; set; } = string.Empty;
        //相机序列号
        public string Sn { get; set; } = string.Empty;
        //相机备注信息
        public string Remark { get; set; } = string.Empty;
        //相机完成信号plc
        public string PlcAddress { get; set; } =    string.Empty;

    }

    public class PlcModel
    {
        //PLC品牌
        public string Brand { set; get; } = string.Empty;
        //PLC通讯协议
        public string Protocol { set; get; } = string.Empty;
        //PLC ip 地址
        public string Ip { set; get; } = string.Empty;
        //PLC端口
        public string Port { set; get; } = string.Empty;
        //PLC 读地址列表
        public List<PLCAddressModels> ReadPLCAddress { get; set; } = new List<PLCAddressModels>();
        //PLC 写地址列表
        public List<PLCAddressModels> WritePLCAddress { get; set; } = new List<PLCAddressModels>();

        public class PLCAddressModels
        {
            //地址名称
            public string Name { get; set; } = string.Empty;
            //地址
            public string Address { get; set; } = string.Empty;
            //地址值
            public string Value { get; set; } = string.Empty;
        }
    }
    public class DatabaseModel
    {
        //数据库 ip 地址
        public string Ip { set; get; } = string.Empty;
        //数据库端口
        public string Port { set; get; } = string.Empty;
        //数据库密码
        public string Password { set; get; } = string.Empty;
        //数据库 库名
        public string LibraryName { set; get; } = string.Empty;
        //二维码表名
        public string CodeTableName { set; get; } = string.Empty;
        //数据表名
        public string DataTableName { set; get; }  = string.Empty;

    }

    public class SolutionModel
    {
        //方案名称
        public string TotalPcs { get; set; } = string.Empty;
        //方案图片数量
        public string TotalImages { get; set; } = string.Empty;
        // 方案排序方式
        public string PcsSorting { get; set; } = string.Empty;
        // 流程步骤列表
        public List<FlowStepModel> FlowSteps { get; set; } = new List<FlowStepModel>();

        public class FlowStepModel
        {
            /// 流程名称
            public string StepName { get; set; } = "未命名流程";
            /// 该流程的PCS数量
            public int Pcs { get; set; } = 0;
            /// 该流程对应的图片索引
            public string ImageIndex { get; set; } = "无";
        }
    }

    public class ImageSaveModel
    {
        // 图像保存路径
        public string ImageSavePath { get; set; } = string.Empty;
        // 图像压缩等级
        public int CompressionLevel { get; set; } = 80; 
    }

}
