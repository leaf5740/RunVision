using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    // 相机模型类
    public class CameraModel
    {
        //相机品牌
        public string Brand { get; set; } = string.Empty;
        //相机序列号
        public string Sn { get; set; } = string.Empty;
        //PLC相机完成地址
        public string PlcAddress { get; set; } = string.Empty;
        //PLC相机完成值
        public string PlcValue { get; set; } = string.Empty;

    }

    // PLC 模型类
    public class PlcModel : BindableBase
    {
        private string _protocol = string.Empty;
        public string Protocol
        {
            get => _protocol;
            set => SetProperty(ref _protocol, value);
        }

        private string _brand = string.Empty;
        public string Brand
        {
            get => _brand;
            set => SetProperty(ref _brand, value);
        }

        private string _ip = string.Empty;
        public string Ip
        {
            get => _ip;
            set => SetProperty(ref _ip, value);
        }

        private string _port = string.Empty;
        public string Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        // PLC 读写地址列表
        public ObservableCollection<PLCAddressModel> PLCAddresses { get; set; } = new ObservableCollection<PLCAddressModel>();

        public class PLCAddressModel : BindableBase
        {
            private string _name = string.Empty;
            public string Name
            {
                get => _name;
                set => SetProperty(ref _name, value);
            }

            private string _address = string.Empty;
            public string Address
            {
                get => _address;
                set => SetProperty(ref _address, value);
            }

            private string _readvalue = string.Empty;
            public string ReadValue
            {
                get => _readvalue;
                set => SetProperty(ref _readvalue, value);
            }

            private string _writeValue = string.Empty;
            public string WriteValue
            {
                get => _writeValue;
                set => SetProperty(ref _writeValue, value);
            }
        }
    }

    // 数据库模型类
    public class DatabaseModel : BindableBase
    {
        // 数据库品牌
        private string _databaseBrand = string.Empty;
        public string DatabaseBrand
        {
            get => _databaseBrand;
            set => SetProperty(ref _databaseBrand, value);
        }

        // 数据库类型
        private string _ip;
        public string Ip
        {
            get => _ip;
            set => SetProperty(ref _ip, value);
        }

        // 数据库端口
        private string _port;
        public string Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        // 数据库用户名
        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        // 数据库密码
        private string _libraryName;
        public string LibraryName
        {
            get => _libraryName;
            set => SetProperty(ref _libraryName, value);
        }

        // code表名称
        private string _codeTableName;
        public string CodeTableName
        {
            get => _codeTableName;
            set => SetProperty(ref _codeTableName, value);
        }

        //数据表名称
        private string _dataTableName;
        public string DataTableName
        {
            get => _dataTableName;
            set => SetProperty(ref _dataTableName, value);
        }
    }

    // 方案模型类
    public class SolutionModel
    {
        // 流程步骤列表
        public List<FlowStepModel> FlowSteps { get; set; } = new List<FlowStepModel>();

        public class FlowStepModel : BindableBase
        {
            private string _stepName;
            public string StepName
            {
                get => _stepName;
                set => SetProperty(ref _stepName, value);
            }

            private string _imageIndex;
            public string ImageIndex
            {
                get => _imageIndex;
                set => SetProperty(ref _imageIndex, value);
            }

            private string _retPcs;
            public string RetPcs
            {
                get => _retPcs;
                set => SetProperty(ref _retPcs, value);
            }
        }
    }

    // 配置模型类
    public class ImageSaveModel : BindableBase
    {
        private string _imageSavePath;
        public string ImageSavePath
        {
            get => _imageSavePath;
            set => SetProperty(ref _imageSavePath, value);
        }

        private string _compressionLevel = "0";
        public string CompressionLevel
        {
            get => _compressionLevel;
            set => SetProperty(ref _compressionLevel, value);
        }
    }

}
