using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunVision.Models
{
    public class ProjectModel
    {
        public List<CameraModel> CamerasConfig { get; set; } = new List<CameraModel>();
        public PlcModel PlcConfig { get; set; } = new PlcModel();
        public DatabaseModel DatabaseConfig { get; set; } = new DatabaseModel();
        public SolutionModel SolutionConfig { get; set; } = new SolutionModel();
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
        public string Brand { set; get; } = string.Empty;
        public string Protocol { set; get; } = string.Empty;
        public string Ip { set; get; } = string.Empty;
        public string Port { set; get; } = string.Empty;

        public List<PLCAddressModels> ReadPLCAddress { get; set; } = new List<PLCAddressModels>();

        public List<PLCAddressModels> WritePLCAddress { get; set; } = new List<PLCAddressModels>();

        public class PLCAddressModels
        {
            public string Name { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public string Remark { get; set; } = string.Empty;

        }
    }
    public class DatabaseModel
    {
        public string Ip { set; get; } = string.Empty;
        public string Port { set; get; } = string.Empty;
        public string Password { set; get; } = string.Empty;
        public string LibraryName { set; get; } = string.Empty;
        public string CodeTableName { set; get; } = string.Empty;
        public string DataTableName { set; get; }  = string.Empty;

    }

    public class SolutionModel
    {
        public string TotalPcs { get; set; } = string.Empty;
        public string TotalImages { get; set; } = string.Empty;
        public string PcsSorting { get; set; }

        public List<FlowStepModel> FlowSteps { get; set; } = new List<FlowStepModel>();

        public class FlowStepModel
        {
            /// <summary>
            /// 流程名称
            /// </summary>
            public string StepName { get; set; } = "未命名流程";

            /// <summary>
            /// 该流程的PCS数量
            /// </summary>
            public int Pcs { get; set; } = 0;

            /// <summary>
            /// 该流程对应的图片索引
            /// </summary>
            public string ImageIndex { get; set; } = "无";

            /// <summary>
            /// 该流程的备注
            /// </summary>
            public string Remark { get; set; } = string.Empty;
        }
    }

    public class ImageSaveModel
    {
        public string ImageSavePath { get; set; } = string.Empty; // 图像保存路径
        public int CompressionLevel { get; set; } = 80; // 图像压缩等级
    }




}
