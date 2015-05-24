using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Configuration;

using SDNUOJ.Configuration.Helper;

namespace SDNUOJ.Configuration
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    public static class ConfigurationManager
    {
        #region 常量
        private const String DEFAULT_ONLINE_JUDGE_NAME = "SDNU Online Judge";
        private const String DEFAULT_UPLOAD_DIRECTORY_PATH = "uploads/";
        private const String DEFAULT_PROBLEM_DATA_PATH = "data/";
        private const String DEFAULT_SUPPORT_LANGUAGES = "0=C=gcc=c=1.0=true|1=C++=g++=cpp=1.0=true";
        private const Int32 DEFAULT_PROBLEMSET_STARTID = 1;
        private const Int32 DEFAULT_SUBMIT_INTERVAL = 0;
        private const Int32 DEFAULT_REGISTER_INTERVAL = 0;
        private const Int32 DEFAULT_CHECK_JUDGE_ERROR_SOLUTION = 0;
        private const Int32 DEFAULT_CHECK_CODE_TIMEOUT = 180;
        private const String DEFAULT_SYSTEM_ACCOUNT = "admin";
        #endregion

        #region 字段
        private static DateTime _systemStartTime;
        private static String _mainVersion;
        private static String _version;
        private static String _revision;

        private static Boolean _systemClosed;
        private static String _closeInformation;
        private static String _onlineJudgeName;
        private static String _domainUrl;
        private static String _uploadDirectoryUrl;
        private static String _problemDataPath;
        private static String _uploadDirectoryPath;
        private static String _supportLanguages;
        private static String _usernameKeywordsFilter;
        private static String _forumKeywordsFilter;
        private static Boolean _loggingEnable;
        private static Boolean _contentCacheEnable;
        private static Boolean _enableJudgerInterface;
        private static Boolean _allowRegister;
        private static Boolean _allowForgetPassword;
        private static Boolean _allowUserControl;
        private static Boolean _allowUserMail;
        private static Boolean _allowUserInfo;
        private static Boolean _allowSourceView;
        private static Boolean _allowMainSubmit;
        private static Boolean _allowMainForum;
        private static Boolean _allowMainProblem;
        private static Boolean _allowMainRanklist;
        private static Boolean _allowMainStatus;
        private static Boolean _allowResource;
        private static Boolean _allowContest;
        private static Boolean _allowDownloadSource;
        private static Boolean _allowMainSubmitStatus;
        private static Boolean _replyPostMailNotification;
        private static Int32 _problemSetStartID;
        private static Int32 _submitInterval;
        private static Int32 _registerInterval;
        private static Int32 _checkJudgeErrorSolution;
        private static Int32 _checkCodeTimeout;
        private static String _recentContestURL;
        private static String _systemAccount;
        private static String _emailSMTPServer;
        private static String _emailUsername;
        private static String _emailPassword;
        private static String _emailAddresser;
        private static String _baiduTongjiID;
        private static Boolean _isDebugMode;
        #endregion

        #region 属性
        /// <summary>
        /// 获取系统启动时间
        /// </summary>
        public static DateTime SystemStartTime
        {
            get { return _systemStartTime; }
        }

        /// <summary>
        /// 获取当前系统主版本号
        /// </summary>
        public static String MainVersion
        {
            get { return _mainVersion; }
        }

        /// <summary>
        /// 获取当前系统版本号
        /// </summary>
        public static String Version
        {
            get { return _version; }
        }

        /// <summary>
        /// 获取当前系统编译次数
        /// </summary>
        public static String Revision
        {
            get { return _revision; }
        }

        /// <summary>
        /// 获取当前系统运行平台
        /// </summary>
        public static String Platform
        {
            get { return "ASP.NET"; }
        }

        /// <summary>
        /// 获取系统名称
        /// </summary>
        public static String OnlineJudgeName
        {
            get { return _onlineJudgeName; }
        }

        /// <summary>
        /// 获取系统是否关闭
        /// </summary>
        public static Boolean SystemClosed
        {
            get { return _systemClosed; }
        }

        /// <summary>
        /// 获取系统关闭时提示信息
        /// </summary>
        public static String CloseInformation
        {
            get { return _closeInformation; }
        }

        /// <summary>
        /// 获取域名地址
        /// </summary>
        public static String DomainUrl
        {
            get { return _domainUrl; }
        }

        /// <summary>
        /// 获取上传目录Url(末尾需要有/)
        /// </summary>
        public static String UploadDirectoryUrl
        {
            get { return _uploadDirectoryUrl; }
        }

        /// <summary>
        /// 获取上传文件路径
        /// </summary>
        public static String UploadDirectoryPath
        {
            get { return _uploadDirectoryPath; }
        }

        /// <summary>
        /// 获取题目数据路径
        /// </summary>
        public static String ProblemDataPath
        {
            get { return _problemDataPath; }
        }

        /// <summary>
        /// 获取程序语言支持
        /// </summary>
        internal static String SupportLanguages
        {
            get { return _supportLanguages; }
        }

        /// <summary>
        /// 获取用户名昵称过滤字符
        /// </summary>
        internal static String UsernameKeywordsFilter
        {
            get { return _usernameKeywordsFilter; }
        }

        /// <summary>
        /// 获取论坛发帖过滤字符
        /// </summary>
        internal static String ForumKeywordsFilter
        {
            get { return _forumKeywordsFilter; }
        }

        /// <summary>
        /// 获取是否启用日志记录
        /// </summary>
        public static Boolean LoggingEnable
        {
            get { return _loggingEnable; }
        }

        /// <summary>
        /// 获取是否启用缓存
        /// </summary>
        public static Boolean ContentCacheEnable
        {
            get { return _contentCacheEnable; }
        }

        /// <summary>
        /// 获取是否启用评测机接口
        /// </summary>
        public static Boolean EnableJudgerInterface
        {
            get { return _enableJudgerInterface; }
        }

        /// <summary>
        /// 获取是否开启注册
        /// </summary>
        public static Boolean AllowRegister
        {
            get { return _allowRegister; }
        }

        /// <summary>
        /// 获取是否开启密码找回
        /// </summary>
        public static Boolean AllowForgetPassword
        {
            get { return _allowForgetPassword; }
        }

        /// <summary>
        /// 获取是否开启用户信息修改
        /// </summary>
        public static Boolean AllowUserControl
        {
            get { return _allowUserControl; }
        }

        /// <summary>
        /// 获取是否开启用户邮箱
        /// </summary>
        public static Boolean AllowUserMail
        {
            get { return _allowUserMail; }
        }

        /// <summary>
        /// 获取是否开启用户信息
        /// </summary>
        public static Boolean AllowUserInfo
        {
            get { return _allowUserInfo; }
        }

        /// <summary>
        /// 获取是否开启代码查看
        /// </summary>
        public static Boolean AllowSourceView
        {
            get { return _allowSourceView; }
        }

        /// <summary>
        /// 获取是否开始主提交
        /// </summary>
        public static Boolean AllowMainSubmit
        {
            get { return _allowMainSubmit; }
        }

        /// <summary>
        /// 获取是否开启主论坛
        /// </summary>
        public static Boolean AllowMainForum
        {
            get { return _allowMainForum; }
        }

        /// <summary>
        /// 获取是否开启主题库
        /// </summary>
        public static Boolean AllowMainProblem
        {
            get { return _allowMainProblem; }
        }

        /// <summary>
        /// 获取是否开启主排行
        /// </summary>
        public static Boolean AllowMainRanklist
        {
            get { return _allowMainRanklist; }
        }

        /// <summary>
        /// 获取是否开启主提交状态
        /// </summary>
        public static Boolean AllowMainStatus
        {
            get { return _allowMainStatus; }
        }

        /// <summary>
        /// 获取是否开启资源下载
        /// </summary>
        public static Boolean AllowResource
        {
            get { return _allowResource; }
        }

        /// <summary>
        /// 获取是否开启竞赛
        /// </summary>
        public static Boolean AllowContest
        {
            get { return _allowContest; }
        }

        /// <summary>
        /// 获取是否允许下载代码
        /// </summary>
        public static Boolean AllowDownloadSource
        {
            get { return _allowDownloadSource; }
        }

        /// <summary>
        /// 获取是否允许查看主提交状态
        /// </summary>
        public static Boolean AllowMainSubmitStatus
        {
            get { return _allowMainSubmitStatus; }
        }

        /// <summary>
        /// 获取回复帖子是否邮件通知
        /// </summary>
        public static Boolean ReplyPostMailNotification
        {
            get { return _replyPostMailNotification; }
        }

        /// <summary>
        /// 获取题目起始ID
        /// </summary>
        public static Int32 ProblemSetStartID
        {
            get { return _problemSetStartID; }
        }

        /// <summary>
        /// 获取提交间隔
        /// </summary>
        public static Int32 SubmitInterval
        {
            get { return _submitInterval; }
        }

        /// <summary>
        /// 获取单IP注册间隔
        /// </summary>
        public static Int32 RegisterInterval
        {
            get { return _registerInterval; }
        }

        /// <summary>
        /// 获取检测提交错误间隔时间(秒)
        /// </summary>
        public static Int32 CheckJudgeErrorSolution
        {
            get { return _checkJudgeErrorSolution; }
        }

        /// <summary>
        /// 获取验证码超时时间(秒)
        /// </summary>
        public static Int32 CheckCodeTimeout
        {
            get { return _checkCodeTimeout; }
        }

        /// <summary>
        /// 获取最近竞赛获取地址
        /// </summary>
        public static String RecentContestURL
        {
            get { return _recentContestURL; }
        }

        /// <summary>
        /// 获取系统账号名称
        /// </summary>
        public static String SystemAccount
        {
            get { return _systemAccount; }
        }

        /// <summary>
        /// 获取电子邮件发送服务器
        /// </summary>
        public static String EmailSMTPServer
        {
            get { return _emailSMTPServer; }
        }

        /// <summary>
        /// 获取电子邮件发送用户名
        /// </summary>
        public static String EmailUsername
        {
            get { return _emailUsername; }
        }

        /// <summary>
        /// 获取电子邮件发送用户密码
        /// </summary>
        public static String EmailPassword
        {
            get { return _emailPassword; }
        }

        /// <summary>
        /// 获取电子邮件发送显示名
        /// </summary>
        public static String EmailAddresser
        {
            get { return _emailAddresser; }
        }

        /// <summary>
        /// 获取百度统计ID
        /// </summary>
        public static String BaiduTongjiID
        {
            get { return _baiduTongjiID; }
        }

        /// <summary>
        /// 获取是否是调试模式
        /// </summary>
        public static Boolean IsDebugMode
        {
            get { return _isDebugMode; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化系统变量
        /// </summary>
        static ConfigurationManager()
        {
            _systemStartTime = DateTime.Now;

            _mainVersion = String.Format("v{0}.{1}", Assembly.GetExecutingAssembly().GetName().Version.Major.ToString(), Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString());
            _version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _revision = Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString();

            ConfigurationManager.InitConfig();
        }

        private static void InitConfig()
        {
            NameValueCollection col = GetConfigCollection();

            //读取配置文件
            _onlineJudgeName = col["OnlineJudgeName"];
            _closeInformation = col["CloseInformation"];
            _domainUrl = col["DomainUrl"];
            _uploadDirectoryUrl = col["UploadDirectoryUrl"];
            _uploadDirectoryPath = col["UploadDirectoryPath"];
            _problemDataPath = col["ProblemDataPath"];
            _supportLanguages = col["SupportLanguages"];
            _usernameKeywordsFilter = col["UsernameKeywordsFilter"];
            _forumKeywordsFilter = col["ForumKeywordsFilter"];

            _systemClosed = "true".Equals(col["SystemClosed"], StringComparison.OrdinalIgnoreCase);
            _loggingEnable = "true".Equals(col["LoggingEnable"], StringComparison.OrdinalIgnoreCase);
            _contentCacheEnable = "true".Equals(col["ContentCacheEnable"], StringComparison.OrdinalIgnoreCase);
            _enableJudgerInterface = "true".Equals(col["EnableJudgerInterface"], StringComparison.OrdinalIgnoreCase);
            _allowRegister = "true".Equals(col["AllowRegister"], StringComparison.OrdinalIgnoreCase);
            _allowForgetPassword = "true".Equals(col["AllowForgetPassword"], StringComparison.OrdinalIgnoreCase);
            _allowUserControl = "true".Equals(col["AllowUserControl"], StringComparison.OrdinalIgnoreCase);
            _allowUserMail = "true".Equals(col["AllowUserMail"], StringComparison.OrdinalIgnoreCase);
            _allowUserInfo = "true".Equals(col["AllowUserInfo"], StringComparison.OrdinalIgnoreCase);
            _allowSourceView = "true".Equals(col["AllowSourceView"], StringComparison.OrdinalIgnoreCase);
            _allowMainSubmit = "true".Equals(col["AllowMainSubmit"], StringComparison.OrdinalIgnoreCase);
            _allowMainForum = "true".Equals(col["AllowMainForum"], StringComparison.OrdinalIgnoreCase);
            _allowMainProblem = "true".Equals(col["AllowMainProblem"], StringComparison.OrdinalIgnoreCase);
            _allowMainRanklist = "true".Equals(col["AllowMainRanklist"], StringComparison.OrdinalIgnoreCase);
            _allowMainStatus = "true".Equals(col["AllowMainStatus"], StringComparison.OrdinalIgnoreCase);
            _allowResource = "true".Equals(col["AllowResource"], StringComparison.OrdinalIgnoreCase);
            _allowContest = "true".Equals(col["AllowContest"], StringComparison.OrdinalIgnoreCase);
            _allowDownloadSource = "true".Equals(col["AllowDownloadSource"], StringComparison.OrdinalIgnoreCase);
            _allowMainSubmitStatus = "true".Equals(col["AllowMainSubmitStatus"], StringComparison.OrdinalIgnoreCase);

            _replyPostMailNotification = "true".Equals(col["ReplyPostMailNotification"], StringComparison.OrdinalIgnoreCase);
            _recentContestURL = col["RecentContestURL"];
            _systemAccount = col["SystemAccount"];
            _emailSMTPServer = col["EmailSMTPServer"];
            _emailUsername = col["EmailUsername"];
            _emailPassword = col["EmailPassword"];
            _emailAddresser = col["EmailAddresser"];

            _baiduTongjiID = col["BaiduTongjiID"];

            Int32.TryParse(col["ProblemSetStartID"], out _problemSetStartID);
            Int32.TryParse(col["SubmitInterval"], out _submitInterval);
            Int32.TryParse(col["RegisterInterval"], out _registerInterval);
            Int32.TryParse(col["CheckJudgeErrorSolution"], out _checkJudgeErrorSolution);
            Int32.TryParse(col["CheckCodeTimeout"], out _checkCodeTimeout);

            //关键配置判断是否有内容
            if (String.IsNullOrEmpty(_onlineJudgeName)) _onlineJudgeName = DEFAULT_ONLINE_JUDGE_NAME;
            if (String.IsNullOrEmpty(_uploadDirectoryPath)) _uploadDirectoryPath = DEFAULT_UPLOAD_DIRECTORY_PATH;
            if (String.IsNullOrEmpty(_problemDataPath)) _problemDataPath = DEFAULT_PROBLEM_DATA_PATH;
            if (String.IsNullOrEmpty(_supportLanguages)) _supportLanguages = DEFAULT_SUPPORT_LANGUAGES;
            if (String.IsNullOrEmpty(_systemAccount)) _systemAccount = DEFAULT_SYSTEM_ACCOUNT;

            if (_problemSetStartID < 0) _problemSetStartID = DEFAULT_PROBLEMSET_STARTID;
            if (_submitInterval < 0) _submitInterval = DEFAULT_SUBMIT_INTERVAL;
            if (_registerInterval < 0) _registerInterval = DEFAULT_REGISTER_INTERVAL;
            if (_checkJudgeErrorSolution < 0) _checkJudgeErrorSolution = DEFAULT_CHECK_JUDGE_ERROR_SOLUTION;
            if (_checkCodeTimeout <= 0) _checkCodeTimeout = DEFAULT_CHECK_CODE_TIMEOUT;

            //赋值绝对路径
            _uploadDirectoryPath = FileHelper.GetRealPath(_uploadDirectoryPath);
            _problemDataPath = FileHelper.GetRealPath(_problemDataPath);

            //获取编译参数
            CompilationSection compilationSettings = GetCompilationSection();
            _isDebugMode = compilationSettings == null ? false : compilationSettings.Debug;
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <returns>配置信息</returns>
        public static NameValueCollection GetConfigCollection()
        {
            return System.Configuration.ConfigurationManager.AppSettings;
        }

        /// <summary>
        /// 获取编译配置信息
        /// </summary>
        /// <returns>编译配置信息</returns>
        public static CompilationSection GetCompilationSection()
        {
            return System.Configuration.ConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
        }

        /// <summary>
        /// 保存配置到配置文件
        /// </summary>
        /// <param name="col">配置信息</param>
        public static void SaveToConfig(NameValueCollection col)
        {
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpRuntime.AppDomainAppVirtualPath);
            Boolean isChanged = false;

            foreach (String key in col.AllKeys)
            {
                if (config.AppSettings.Settings[key] != null && !config.AppSettings.Settings[key].Value.Equals(col[key]))
                {
                    config.AppSettings.Settings[key].Value = col[key];
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                config.Save();
                ConfigurationManager.InitConfig();
            }
        }
        #endregion
    }
}