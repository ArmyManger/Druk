using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.WeChat.Util
{
    #region 第三方平台授权方对象

    #region 第三方平台

    public class authorization
    {

        /// <summary>
        /// 授权信息
        /// </summary>
        public authorization_info authorization_info;
    }

    /// <summary>
    /// 授权信息
    /// </summary>
    public class authorization_info
    {
        /// <summary>
        /// 授权的AppID
        /// </summary>
        public string authorizer_appid;
        /// <summary>
        /// 授权的access_token
        /// </summary>
        public string authorizer_access_token;

        /// <summary>
        /// 有效期
        /// </summary>
        public int expires_in;

        /// <summary>
        /// 授权拿到的refresh_token
        /// </summary>
        public string authorizer_refresh_token;

        /// <summary>
        /// 授权的权限
        /// </summary>
        public List<funcscope_categoryItem> func_info;
    }
    public class funcscope_categoryItem
    {
        public funcscope_category funcscope_category;
    }


    public class funcscope_category
    {
        public int id;
    }


    public class ComponentInfo
    {

        /// <summary>
        /// 第三方平台的AppSecret
        /// </summary>
        public string ComponentAppSecret { get; set; }

        /// <summary>
        /// 第三方票证
        /// </summary>
        public string ComponentVerifyTicket { get; set; }

        /// <summary>
        /// 第三方平台消息加解密Key
        /// </summary>
        public string ComponentEncodingAESKey { get; set; }

        /// <summary>
        /// 第三方平台消息校验的Token
        /// </summary>
        public string ComponentToken { get; set; }

        /// <summary>
        /// 第三方平台的AccessToken
        /// </summary>
        public string ComponentAccessToken { get; set; }
    }

    public class ComponentSimple
    {
        /// <summary>
        /// 第三方平台 的 appid
        /// </summary>
        public string componentAppId { get; set; }

        /// <summary>
        /// 第三方平台的AccessToken
        /// </summary>
        public string accessToken { get; set; }

        /// <summary>
        /// 小程序的appid
        /// </summary>
        public string appId { get; set; }
    }



    public class ApiToken
    {


        /// <summary>
        /// 新token，token失效后，可用refreshToken刷新获取新的token
        /// </summary>
        public string refreshToken { get; set; }

        /// <summary>
        ///  接口token （有效期2小时）
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// token过期时间
        /// </summary>
        public DateTime tokenExpires { get; set; }
    }


    #endregion

    public class GetAuthorizerInfoResult : WxJsonResult
    {
        /// <summary>
        /// 授权方信息
        /// </summary>
        public AuthorizerInfo authorizer_info { get; set; }

        /// <summary>
        /// 授权信息（此对象返回的数据不全，请使用盛派的 ComponentApi.QueryAuth接口）
        /// </summary>
        public AuthorizationInfo authorization_info { get; set; }
    }
    public class AccessTokenSimple
    {
        public string accessToken { get; set; }
    }
    /// <summary>
    /// 授权方信息
    /// </summary>
    public class AuthorizerInfo
    {
        /// <summary>
        /// 授权方昵称
        /// </summary>
        public string nick_name { get; set; }
        /// <summary>
        /// 授权方头像
        /// </summary>
        public string head_img { get; set; }
        /// <summary>
        /// 授权方公众号类型，0代表订阅号(小程序默认也是0，用MiniProgramInfo来判断是否是小程序)，1代表由历史老帐号升级后的订阅号，2代表服务号
        /// </summary>
        public Service_Type_Info service_type_info { get; set; }
        /// <summary>
        /// 授权方认证类型，-1代表未认证，0代表微信认证，1代表新浪微博认证，2代表腾讯微博认证，3代表已资质认证通过但还未通过名称认证，4代表已资质认证通过、还未通过名称认证，但通过了新浪微博认证，5代表已资质认证通过、还未通过名称认证，但通过了腾讯微博认证
        /// </summary>
        public Verify_Type_Info verify_type_info { get; set; }
        /// <summary>
        /// 授权方公众号的原始ID
        /// </summary>
        public string user_name { get; set; }
        /// <summary>
        /// 授权方公众号所设置的微信号，可能为空
        /// </summary>
        public string alias { get; set; }
        /// <summary>
        /// 二维码图片的URL，开发者最好自行也进行保存
        /// </summary>
        public string qrcode_url { get; set; }

        /// <summary>
        /// 用以了解以下功能的开通状况（0代表未开通，1代表已开通）： open_store:是否开通微信门店功能 open_scan:是否开通微信扫商品功能 open_pay:是否开通微信支付功能 open_card:是否开通微信卡券功能 open_shake:是否开通微信摇一摇功能
        /// </summary>
        public Business_Info business_info { get; set; }
        public int idc { get; set; }
        /// <summary>
        /// 公众号的主体名称
        /// </summary>
        public string principal_name { get; set; }

        public string signature { get; set; }

        /// <summary>
        /// 小程序相关信息
        /// </summary>
        public Miniprograminfo MiniProgramInfo { get; set; }
    }


    /// <summary>
    /// 授权方公众号类型
    /// </summary>
    public class Service_Type_Info
    {
        public int id { get; set; }
    }

    /// <summary>
    /// 授权方认证类型
    /// </summary>
    public class Verify_Type_Info
    {
        public int id { get; set; }
    }


    /// <summary>
    /// 用以了解以下功能的开通状况（0代表未开通，1代表已开通）： open_store:是否开通微信门店功能 open_scan:是否开通微信扫商品功能 open_pay:是否开通微信支付功能 open_card:是否开通微信卡券功能 open_shake:是否开通微信摇一摇功能
    /// </summary>
    public class Business_Info
    {
        public int open_pay { get; set; }
        public int open_shake { get; set; }
        /// <summary>
        /// 是否开通微信扫商品功能
        /// </summary>
        public int open_scan { get; set; }
        public int open_card { get; set; }
        public int open_store { get; set; }
    }

    /// <summary>
    /// 小程序相关信息
    /// </summary>
    public class Miniprograminfo
    {

        /// <summary>
        /// 小程序已设置的各个服务器域名
        /// </summary>
        public Network network { get; set; }
        public object[] categories { get; set; }
        public int visit_status { get; set; }
    }


    /// <summary>
    /// 小程序已设置的各个服务器域名
    /// </summary>
    public class Network
    {
        /// <summary>
        /// request合法域名
        /// </summary>
        public string[] RequestDomain { get; set; }

        /// <summary>
        /// socket合法域名
        /// </summary>
        public string[] WsRequestDomain { get; set; }

        /// <summary>
        /// uploadFile合法域名
        /// </summary>
        public string[] UploadDomain { get; set; }
        /// <summary>
        /// downloadFile合法域名
        /// </summary>
        public string[] DownloadDomain { get; set; }
        public string[] BizDomain { get; set; }
    }

    /// <summary>
    /// 授权信息
    /// </summary>
    public class AuthorizationInfo
    {
        /// <summary>
        /// 授权方appid
        /// </summary>
        public string authorizer_appid { get; set; }
        /// <summary>
        /// 授权方令牌（在授权的公众号具备API权限时，才有此返回值）
        /// </summary>
        public string authorizer_refresh_token { get; set; }

        /// <summary>
        /// 授权给开发者的权限集列表
        /// ID为1到15时分别代表：
        /// 1.消息管理权限
        /// 2.用户管理权限 
        /// 3.帐号服务权限 
        /// 4.网页服务权限 
        /// 5.微信小店权限 
        /// 6.微信多客服权限 
        /// 7.群发与通知权限
        /// 8.微信卡券权限 
        /// 9.微信扫一扫权限 
        /// 10.微信连WIFI权限 
        /// 11.素材管理权限 
        /// 12.微信摇周边权限 
        /// 13.微信门店权限 
        /// 14.微信支付权限 
        /// 15.自定义菜单权限
        /// 17.小程序帐号管理权限 
        /// 18.小程序开发管理权限 
        /// 19.小程序客服消息管理权限 
        /// </summary>
        public Func_Info[] func_info { get; set; }
    }


    /// <summary>
    /// 授权给开发者的权限集列表
    /// </summary>
    public class Func_Info
    {
        public Funcscope_Category funcscope_category { get; set; }
        public Confirm_Info confirm_info { get; set; }
    }

    public class Funcscope_Category
    {
        public int id { get; set; }
    }

    public class Confirm_Info
    {
        public int need_confirm { get; set; }
        public int already_confirm { get; set; }
        public int can_confirm { get; set; }
    }

    #endregion
     
    #region 公众号相关
    #region 公众号粉丝

    /// <summary>
    /// 粉丝列表
    /// </summary>
    public class FansList
    {

        /// <summary>
        /// 关注该公众账号的总用户数
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 拉取的OPENID个数，最大值为10000
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 拉取列表的最后一个用户的OPENID
        /// </summary>
        public string nextOpenId { get; set; }

        /// <summary>
        /// 列表数据，OPENID的列表
        /// </summary>
        public List<string> openId { get; set; }
    }


    /// <summary>
    /// 用户明文信息
    /// </summary>
    public class FansInfo
    {
        public int wxId { get; set; }
        public string wxName { get; set; }

        public string openId { get; set; }
        public string unionId { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string nickName { get; set; }//   
        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        public string gender { get; set; }//  
        /// <summary>
        ///   用户的语言，简体中文为zh_CN
        /// </summary>
        public string language { get; set; }//

        /// <summary>
        /// 用户所在城市
        /// </summary>
        public string city { get; set; }// 

        /// <summary>
        /// 用户所在省份
        /// </summary>
        public string province { get; set; }//  

        /// <summary>
        /// 用户所在国家
        /// </summary>
        public string country { get; set; }// 

        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表132*132正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
        /// </summary>
        public string avatarUrl { get; set; }// 

    }


    /// <summary>
    /// 批量获取用户基本信息数据
    /// </summary>
    public class BatchGetUserInfoData
    {
        public BatchGetUserInfoData()
        {
            this.lang = "zh_CN";
        }
        /// <summary>
        ///  用户的标识，对当前公众号唯一 必填
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// 国家地区语言版本，请使用Language范围内的值：zh_CN 简体，zh_TW 繁体，en 英语，默认为zh-CN 非必填
        /// </summary>
        public string lang { get; set; }

    }
    #endregion

    #region 公众号微信图文

    /// <summary>
    /// 微信图文
    /// </summary>
    public class WechatNewsModel
    {

        /// <summary>
        ///  图文消息缩略图的media_id，可以在基础支持上传多媒体文件接口中获得
        /// </summary>
        public string thumb_media_id { get; set; }


        /// <summary>
        ///   图文消息的作者
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// 图文消息的标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 在图文消息页面点击“阅读原文”后的页面
        /// </summary>
        public string content_source_url { get; set; }

        /// <summary>
        /// 图文消息页面的内容，支持HTML标签
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 图文消息的描述
        /// </summary>
        public string digest { get; set; }

        /// <summary>
        ///  是否显示封面，1为显示，0为不显示
        /// </summary>
        public string show_cover_pic { get; set; }

        /// <summary>
        /// 缩略图的URL
        /// </summary>
        public string thumb_url { get; set; }


        /// <summary>
        ///  是否打开评论，0不打开，1打开
        /// </summary>
        public int need_open_comment { get; set; }

        /// <summary>
        /// 是否粉丝才可评论，0所有人可评论，1粉丝才可评论
        /// </summary>
        public int only_fans_can_comment { get; set; }


        /// <summary>
        /// 图文页的URL
        /// </summary>
        public string url { get; set; }
    }
    #endregion

    #region 公众号微信接口返回的基础字段

    /// <summary>
    /// 微信接口返回的基础字段
    /// </summary>
    public class WxJsonResult
    {
        /// <summary>
        /// 返回消息代码数字
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 返回结果信息
        /// </summary>
        public string errmsg { get; set; }

        public object P2PData { get; set; }
    }

    public class WxJsonDepResult
    {
        /// <summary>
        /// 返回消息代码数字
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 返回结果信息
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 创建的部门id
        /// </summary>
        public int id { get; set; }
    }
    #endregion

    #region 公众号模板消息
    /// <summary>
    /// 公众号发送模板消息所需要的model
    /// </summary>
    public class TemplateModel : Template
    {



        /// <summary>
        /// 模板消息的跳转方式 （0不跳转 1url 2小程序）
        /// </summary>
        public int jumpMode { get; set; }



        /// <summary>
        /// 所需跳转到的小程序appid（该小程序appid必须与发模板消息的公众号是绑定关联关系，暂不支持小游戏）
        /// </summary>
        public string miniAppId { get; set; }

        /// <summary>
        /// 所需跳转到小程序的具体页面路径，支持带参数,（示例index?foo=bar），暂不支持小游戏
        /// </summary>
        public string miniPagePath { get; set; }
    }

    public class Template
    {

        /// <summary>
        /// 消息模板的id（系统的id）
        /// </summary>
        public int templateId { get; set; }

        /// <summary>
        ///  接收者（用户）的 openid（注意：发公众号模板消息时为公众号的对应的openid；发小程序的服务通知时为小程序对应的openid）
        /// </summary>
        public string openId { get; set; }


        /// <summary>
        /// 会员的id，openId为空时必传，会根据会员的unionid找到对应微信的openid
        /// </summary>
        public int memberId { get; set; }


        /// <summary>
        /// 模板消息需要处理的内容，key为占位符，value为占位符的值
        /// </summary>
        public Dictionary<string, string> contentValue { get; set; }

        /// <summary>
        /// 模板消息的跳转链接（公众号外链必须是完整的url；小程序点击模板卡片后的跳转页面，仅限本小程序内的页面，支持带参数,（示例index?foo=bar）不填则模板无跳转。）
        /// </summary>
        public string pageUrl { get; set; }
    }


    /// <summary>
    /// 小程序发送模板消息所需要的model
    /// </summary>
    public class TemplateMiniEntity : Template
    {

        /// <summary>
        /// 表单提交场景下，为 submit 事件带上的 formId；支付场景下，为本次支付的 prepay_id
        /// </summary>
        public string formId { get; set; }


    }


    /// <summary>
    /// 小程序模板原生对象
    /// </summary>
    public class TemplateWxEntity
    {
        /// <summary>
        /// 接口调用凭证
        /// </summary>
        public string accessToken { get; set; }
        /// <summary>
        /// 接收者（用户）的 openid
        /// </summary>
        public string openId { get; set; }
        /// <summary>
        /// 所需下发的微信模板消息的id
        /// </summary>
        public string WxTemplateId { get; set; }
        /// <summary>
        /// 模板内容，不填则下发空模板
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// 订阅消息体
        /// </summary>

        //   public Senparc.Weixin.Entities.TemplateMessage.TemplateMessageData subscribeData { get; set; }

        /// <summary>
        /// 表单提交场景下，为 submit 事件带上的 formId；支付场景下，为本次支付的 prepay_id
        /// </summary>
        public string formId { get; set; }
        /// <summary>
        /// 点击模板卡片后的跳转页面，仅限本小程序内的页面。支持带参数,（示例index?foo=bar）。该字段不填则模板无跳转。
        /// </summary>
        public string page { get; set; }
        /// <summary>
        /// 模板需要放大的关键词，不填则默认无放大
        /// </summary>
        public string emphasisKeyword { get; set; }
    }
    #endregion

    #region 公众号网页授权的用户信息


    /// <summary>
    /// 通过OAuth的获取到的用户信息（scope=snsapi_userinfo 其他字段才会有值，否则只有openid有值）
    /// </summary>
    public class OAuthUserInfo
    {


        public string openid { get; set; }
        public string nickname { get; set; }

        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        public int sex { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
        /// </summary>
        public string headimgurl { get; set; }

        /// <summary>
        /// 用户特权信息，json 数组，如微信沃卡用户为（chinaunicom） 作者注：其实这个格式称不上JSON，只是个单纯数组。
        /// </summary>
        public string[] privilege { get; set; }
        public string unionid { get; set; }
    }
    #endregion


    #endregion

    #region 企业号相关

    /// <summary>
    /// 企业微信环境小程序登录凭证
    /// </summary>
    public class JsCode2Session : WxJsonResult
    {

        /// <summary>
        /// 用户所属企业的corpid
        /// </summary>
        public string corpid { get; set; }

        /// <summary>
        /// 用户在企业内的UserID，对应管理端的帐号，企业内唯一。注意：如果该企业没有关联该小程序，则此处返回加密的userid
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// 会话密钥
        /// </summary>
        public string session_key { get; set; }

    }

    /// <summary>
    /// 企业号部门
    /// </summary>
    public class WorkDepartment
    {

        /// <summary>
        ///部门id，32位整型，指定时必须大于1。若不填该参数，将自动生成id
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 部门名称。长度限制为1~32个字符，字符不能包括\:?”<>｜
        /// </summary>
        public string name { get; set; }

        /// <summary>
        ///  父部门id，32位整型
        /// </summary>
        public long parentid { get; set; }

        /// <summary>
        /// 在父部门中的次序值。order值大的排序靠前。有效的值范围是[0, 2^32)
        /// </summary>
        public long order { get; set; }

        /// <summary>
        /// 子级列表
        /// </summary>
        public List<WorkDepartment> child { get; set; }
    }


    public class WorkDepartmentMemberList : WxJsonResult
    {

        public List<WorkDepartmentMember> userlist { get; set; }
    }

    public class WorkDepartmentMember
    {
        /// <summary>
        ///  扩展属性
        /// </summary>
        public Extattr extattr { get; set; }
        /// <summary>
        ///  英文名。第三方暂不支持
        /// </summary>
        public string english_name { get; set; }
        /// <summary>
        /// 座机。第三方暂不支持
        /// </summary>
        public string telephone { get; set; }
        /// <summary>
        /// 激活状态: 1=已激活，2=已禁用，4=未激活 已激活代表已激活企业微信或已关注微信插件。未激活代表既未激活企业微信又未关注微信插件。
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 头像url。注：小图将url最后的"/0"改成"/64"
        /// </summary>
        public string avatar { get; set; }

        /// <summary>
        /// 上级字段，标识是否为上级。第三方暂不支持
        /// </summary>
        public int isleader { get; set; }


        /// <summary>
        /// 启用/禁用成员，第三方不可获取。1表示启用成员，0表示禁用成员
        /// </summary>
        public int enable { get; set; }

        /// <summary>
        ///  邮箱
        /// </summary>
        public string email { get; set; }

        /// <summary>
        ///  手机号码
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        ///  职位信息
        /// </summary>
        public string position { get; set; }

        /// <summary>
        ///  部门内的排序值，默认为0。数量必须和department一致，数值越大排序越前面。值范围是[0, 2^32)
        /// </summary>
        public int[] order { get; set; }

        /// <summary>
        /// 成员所属部门id列表
        /// </summary>
        public long[] department { get; set; }


        /// <summary>
        ///  成员名称
        /// </summary>
        public string name { get; set; }


        /// <summary>
        ///   员工UserID
        /// </summary>
        public string userid { get; set; }


        /// <summary>
        ///  性别。gender=0表示男，=1表示女
        /// </summary>
        public int gender { get; set; }


        /// <summary>
        /// 关注微信插件的状态: 1=已关注，0=未关注
        /// </summary>
        public string wxplugin_status { get; set; }


        /// <summary>
        /// 是否隐藏手机号 1是0否
        /// </summary>

        public int hide_mobile { get; set; }

        /// <summary>
        /// 员工个人二维码，扫描可添加为外部联系人
        /// </summary>
        public string qr_code { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string alias { get; set; }

        /// <summary>
        /// 示在所在的部门内是否为上级；第三方仅通讯录应用可获取
        /// </summary>
        public int[] is_leader_in_dept { get; set; }


    }
    /// <summary>
    /// 扩展属性
    /// </summary>
    public class Extattr
    {
        public List<Attr> attrs { get; set; }
    }

    /// <summary>
    /// 扩展属性
    /// </summary>
    public class Attr
    {

        public string name { get; set; }
        public string value { get; set; }
    }
    #endregion

    #region 小程序相关

    #region 小程序 Js Code SessionKey
    /// <summary>
    /// JsCode2Json接口结果
    /// </summary>
    public class JsCode2JsonResult : WxJsonResult
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 会话密钥
        /// </summary>
        public string session_key { get; set; }
    }
    #endregion


    #region 模板消息相关
    public class MiniTemplateEntity
    {


        /// <summary>
        /// 接收者（用户）的 openid
        /// </summary>
        public string touser { get; set; }

        /// <summary>
        /// 所需下发的模板消息的id
        /// </summary>

        public string template_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string page { get; set; }

        /// <summary>
        /// 表单提交场景下，为 submit 事件带上的 formId；支付场景下，为本次支付的 prepay_id
        /// </summary>

        public string form_id { get; set; }

        /// <summary>
        /// 模板内容，不填则下发空模板。具体格式请参考示例。
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 模板需要放大的关键词，不填则默认无放大
        /// </summary>
        public string emphasis_keyword { get; set; }

    }
    #endregion

    #endregion

    #region 通讯录相关
    public class User
    {
        /// <summary>
        /// 成员UserId
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 成员名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 成员别名
        /// </summary>
        public string alias { get; set; }
        /// <summary>
        /// 成员手机号
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 成员所属部门id
        /// </summary>
        public long[] department { get; set; }
        /// <summary>
        /// 部门内的排序值
        /// </summary>
        public int[] order { get; set; }
        /// <summary>
        /// 职务信息
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 性别(1表示男性  2标识女性)
        /// </summary>
        public int gender { get; set; }
        /// <summary>
        /// 成员邮箱
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 座机
        /// </summary>
        public string telephone { get; set; }
        /// <summary>
        /// 个数必须和department一致，表示在所在的部门内是否为上级。1表示为上级，0表示非上级。在审批等应用里可以用来标识上级审批人
        /// </summary>
        public int[] is_leader_in_dept { get; set; }
        /// <summary>
        /// 成员头像的mediaid
        /// </summary>
        public string avatar_mediaid { get; set; }
        /// <summary>
        /// 启用/禁用成员。1表示启用成员，0表示禁用成员
        /// </summary>
        public int enable { get; set; }
        /// <summary>
        /// 自定义字段。自定义字段需要先在WEB管理端添加
        /// </summary>
        public extattr extattr { get; set; }
        /// <summary>
        /// 是否邀请该成员使用企业微信（将通过微信服务通知或短信或邮件下发邀请，每天自动下发一次，最多持续3个工作日），默认值为true。
        /// </summary>
        public bool to_invite { get; set; }
        /// <summary>
        /// 成员对外属性
        /// </summary>
        public external_profile external_profile { get; set; }
        /// <summary>
        /// 成员对外职务
        /// </summary>
        public string external_position { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }
    }
    public class extattr
    {
        public List<attrs> attrs { get; set; }
    }
    public class attrs
    {
        public int type { get; set; }
        public string name { get; set; }
        public text text { get; set; }
    }
    public class text
    {
        public string value { get; set; }
    }
    public class external_profile
    {
        public string external_corp_name { get; set; }
        public List<external_attr> external_attr { get; set; }
    }
    public class external_attr
    {
        public int type { get; set; }
        public string name { get; set; }
        public text text { get; set; }
    }
    public class UserList : User
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }
    public class UserIdList
    {
        public string[] useridlist { get; set; }
    }
    public class Department
    {
        /// <summary>
        /// 部门名称。长度限制为1~32个字符，字符不能包括\:?”<>｜
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 英文名称，需要在管理后台开启多语言支持才能生效。长度限制为1~32个字符，字符不能包括\:?”<>｜
        /// </summary>
        public string name_en { get; set; }
        /// <summary>
        /// 父部门id，32位整型
        /// </summary>
        public long parentid { get; set; }
        /// <summary>
        /// 在父部门中的次序值。order值大的排序靠前。有效的值范围是[0, 2^32)
        /// </summary>
        public long order { get; set; }
        /// <summary>
        /// 部门id，32位整型，指定时必须大于1。若不填该参数，将自动生成id
        /// </summary>
        public long id { get; set; }
    }
    public class DepartmentsList
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public List<Department> department { get; set; }
    }
    public class InviteResult
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public string invaliduser { get; set; }
        public string invalidparty { get; set; }
        public string invalidtag { get; set; }
    }
    public class Invite
    {
        public string[] user { get; set; }
        public string[] party { get; set; }
        public string[] tag { get; set; }
    }
    public class Tag
    {
        public string tagname { get; set; }
        public int tagid { get; set; }
    }
    public class TagResult
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public List<Tag> taglist { get; set; }
        public int tagid { get; set; }
    }
    public class TagUser
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string tagname { get; set; }
        public List<Tuser> userlist { get; set; }
    }
    public class Tuser
    {
        public string userid { get; set; }
        public string name { get; set; }
    }
    public class AddTagUser
    {
        public int tagid { get; set; }
        public string[] userlist { get; set; }
        public string[] partylist { get; set; }
    }
    #endregion

    /// <summary>
    /// 小程序二维码
    /// </summary>
    public class MiniQrCode
    {

        /// <summary>
        /// 场景
        /// </summary>
        public string scene { get; set; }
        /// <summary>
        /// 分页大小
        /// </summary>
        public string page { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public bool auto_color { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object line_color { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool is_hyaline { get; set; }
    }
}
