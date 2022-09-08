using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Druk.WeChat.TenPay
{ 
    /// <summary>
    /// 发起支付的对象,如jsapi 、小程序等
    /// </summary>
    public class PaymentResult
    {

        /// <summary>
        /// 支付账号的appid
        /// </summary>
        public string appId { set; get; }

        /// <summary>
        ///  微信生成的预支付ID，用于后续接口调用中使用
        /// </summary>
        public string prepayId { set; get; }


        /// <summary>
        /// 时间戳
        /// </summary>
        public string timeStamp { get; set; }


        /// <summary>
        /// 随机数
        /// </summary>
        public string nonceStr { get; set; }


        /// <summary>
        /// 统一下单接口返回的 prepay_id 参数值，提交格式如：prepay_id=***
        /// </summary>
        public string package { get; set; }


        /// <summary>
        /// 签名方式 默认MD5
        /// </summary>
        public string signType { get; set; }


        /// <summary>
        ///签名
        /// </summary>
        public string paySign { get; set; }
    }

    /// <summary>
    /// 订单查询结果
    /// </summary>
    public class OrderQueryResult : Result
    {
        /// <summary>
        /// 微信支付分配的终端设备号
        /// </summary>
        public string device_info { get; set; }

        /// <summary>
        /// 用户在商户appid下的唯一标识
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// 用户是否关注公众账号，Y-关注，N-未关注，仅在公众账号类型支付有效
        /// </summary>
        public string is_subscribe { get; set; }

        /// <summary>
        /// 用户子标识[服务商]
        /// </summary>
        public string sub_openid { get; set; }

        /// <summary>
        /// 是否关注子公众账号[服务商]
        /// </summary>
        public string sub_is_subscribe { get; set; }

        /// <summary>
        /// 调用接口提交的交易类型，取值如下：JSAPI，NATIVE，APP，MICROPAY
        /// </summary>
        public string trade_type { get; set; }

        /// <summary>
        ///SUCCESS—支付成功
        ///REFUND—转入退款
        ///NOTPAY—未支付
        ///CLOSED—已关闭
        ///REVOKED—已撤销（刷卡支付）
        ///USERPAYING--用户支付中
        ///PAYERROR--支付失败(其他原因，如银行返回失败)
        /// </summary>
        public string trade_state { get; set; }

        /// <summary>
        /// 银行类型，采用字符串类型的银行标识
        /// </summary>
        public string bank_type { get; set; }

        /// <summary>
        /// 商品详情[服务商]
        /// </summary>
        public string detail { get; set; }

        /// <summary>
        /// 订单总金额，单位为分
        /// </summary>
        public string total_fee { get; set; }

        /// <summary>
        /// 应结订单金额=订单金额-非充值代金券金额，应结订单金额<=订单金额
        /// </summary>
        public string settlement_total_fee { get; set; }

        /// <summary>
        /// 货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY
        /// </summary>
        public string fee_type { get; set; }

        /// <summary>
        /// 现金支付金额订单现金支付金额
        /// </summary>
        public string cash_fee { get; set; }

        /// <summary>
        /// 货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY
        /// </summary>
        public string cash_fee_type { get; set; }

        /// <summary>
        /// “代金券”金额<=订单金额，订单金额-“代金券”金额=现金支付金额
        /// </summary>
        public string coupon_fee { get; set; }

        /// <summary>
        /// 代金券使用数量
        /// </summary>
        public string coupon_count { get; set; }

        /// <summary>
        /// CASH--充值代金券 
        ///NO_CASH---非充值代金券
        ///订单使用代金券时有返回（取值：CASH、NO_CASH）。$n为下标,从0开始编号，举例：coupon_type_$0
        ///coupon_type_$n
        /// </summary>
        public IList<string> coupon_type_values { get; set; }

        /// <summary>
        /// 代金券ID, $n为下标，从0开始编号
        /// coupon_id_$n
        /// </summary>
        public IList<string> coupon_id_values { get; set; }

        /// <summary>
        /// 单个代金券支付金额, $n为下标，从0开始编号
        /// coupon_fee_$n
        /// </summary>
        public IList<int> coupon_fee_values { get; set; }

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string transaction_id { get; set; }

        /// <summary>
        /// 商户系统的订单号，与请求一致。
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 附加数据，原样返回
        /// </summary>
        public string attach { get; set; }

        /// <summary>
        /// 订单支付时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010
        /// </summary>
        public string time_end { get; set; }

        /// <summary>
        /// 对当前查询订单状态的描述和下一步操作的指引
        /// </summary>
        public string trade_state_desc { get; set; }


    }

    /// <summary>
    /// 关闭订单结果
    /// </summary>
    public class CloseOrderResult : Result
    {
        /// <summary>
        /// 对于业务执行的详细描述
        /// </summary>
        public string result_msg { get; set; }
    }

    /// <summary>
    /// 申请退款结果
    /// </summary>
    public class RefundResult : Result
    {
        #region 错误代码
        /*
            名称  描述 原因  解决方案
            SYSTEMERROR 接口返回错误 系统超时等   请不要更换商户退款单号，请使用相同参数再次调用API。
        TRADE_OVERDUE 订单已经超过退款期限  订单已经超过可退款的最大期限(支付后一年内可退款)   请选择其他方式自行退款
            ERROR   业务错误 申请退款业务发生错误  该错误都会返回具体的错误原因，请根据实际返回做相应处理。
        USER_ACCOUNT_ABNORMAL 退款请求失败  用户帐号注销 此状态代表退款申请失败，商户可自行处理退款。
        INVALID_REQ_TOO_MUCH 无效请求过多  连续错误请求数过多被系统短暂屏蔽 请检查业务是否正常，确认业务正常后请在1分钟后再来重试
            NOTENOUGH   余额不足 商户可用退款余额不足  此状态代表退款申请失败，商户可根据具体的错误提示做相应的处理。
        INVALID_TRANSACTIONID 无效transaction_id    请求参数未按指引进行填写 请求参数错误，检查原交易号是否存在或发起支付交易接口返回失败
            PARAM_ERROR 参数错误 请求参数未按指引进行填写    请求参数错误，请重新检查再调用退款申请
            APPID_NOT_EXIST APPID不存在 参数中缺少APPID  请检查APPID是否正确
            MCHID_NOT_EXIST MCHID不存在 参数中缺少MCHID  请检查MCHID是否正确
            APPID_MCHID_NOT_MATCH   appid和mch_id不匹配 appid和mch_id不匹配 请确认appid和mch_id是否匹配
            REQUIRE_POST_METHOD 请使用post方法 未使用post传递参数     请检查请求参数是否通过post方法提交
            SIGNERROR   签名错误 参数签名结果不正确   请检查签名参数和方法是否都符合签名算法要求
            XML_FORMAT_ERROR    XML格式错误 XML格式错误 请检查XML参数格式是否正确
            FREQUENCY_LIMITED   频率限制	2个月之前的订单申请退款有频率限制 该笔退款未受理，请降低频率后重试
         */

        #endregion


        /// <summary>
        /// 	微信支付分配的终端设备号，与下单一致
        /// </summary>
        public string device_info { get; set; }

        /// <summary>
        /// 微信订单号
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 商户退款单号	
        /// </summary>
        public string out_refund_no { get; set; }
        /// <summary>
        /// 微信退款单号
        /// </summary>
        public string refund_id { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public string refund_fee { get; set; }
        /// <summary>
        /// 应结退款金额
        /// </summary>
        public string settlement_refund_fee { get; set; }
        /// <summary>
        /// 标价金额
        /// </summary>
        public string total_fee { get; set; }
        /// <summary>
        /// 应结订单金额
        /// </summary>
        public string settlement_total_fee { get; set; }
        /// <summary>
        /// 标价币种
        /// </summary>
        public string fee_type { get; set; }
        /// <summary>
        /// 现金支付金额
        /// </summary>
        public string cash_fee { get; set; }
        /// <summary>
        /// 现金支付币种
        /// </summary>
        public string cash_fee_type { get; set; }
        /// <summary>
        /// 现金退款金额	
        /// </summary>
        public string cash_refund_fee { get; set; }
        /// <summary>
        /// 代金券退款总金额
        /// </summary>
        public string coupon_refund_fee { get; set; }
        /// <summary>
        /// 退款代金券使用数量
        /// </summary>
        public string coupon_refund_count { get; set; }


        #region 带下标参数

        /// <summary>
        /// 代金券类型
        /// </summary>
        public IList<string> coupon_type_n { get; set; }
        /// <summary>
        /// 单个代金券退款金额
        /// </summary>
        public IList<int> coupon_refund_fee_n { get; set; }
        /// <summary>
        /// 退款代金券ID	
        /// </summary>
        public IList<string> coupon_refund_id_n { get; set; }

        #endregion

    }

    /// <summary>
    /// 退款查询结果
    /// </summary>
    public class RefundQueryResult : Result
    {
        /// <summary>
        /// 终端设备号
        /// </summary>
        public string device_info { get; set; }

        /// <summary>
        /// 微信订单号
        /// </summary>
        public string transaction_id { get; set; }

        /// <summary>
        /// 商户系统内部的订单号
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 订单总金额，单位为分，只能为整数
        /// </summary>
        public string total_fee { get; set; }

        /// <summary>
        /// 应结订单金额=订单金额-非充值代金券金额，应结订单金额<=订单金额
        /// </summary>
        public string settlement_total_fee { get; set; }

        /// <summary>
        /// 订单金额货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY
        /// </summary>
        public string fee_type { get; set; }

        /// <summary>
        /// 现金支付金额，单位为分，只能为整数
        /// </summary>
        public string cash_fee { get; set; }

        /// <summary>
        /// 退款记录数
        /// </summary>
        public string refund_count { get; set; }


        /// <summary>
        /// REFUND_SOURCE_RECHARGE_FUNDS---可用余额退款/基本账户
        //REFUND_SOURCE_UNSETTLED_FUNDS---未结算资金退款
        /// </summary>
        public string refund_account { get; set; }

        /// 退款状态：
        ///SUCCESS—退款成功
        ///FAIL—退款失败
        ///PROCESSING—退款处理中
        ///CHANGE—转入代发，退款到银行发现用户的卡作废或者冻结了，导致原路退款银行卡失败，资金回流到商户的现金帐号，需要商户人工干预，通过线下或者财付通转账的方式进行退款。
        /// </summary>
        public string refund_status { get; set; }

    }

    /// <summary>
    /// 撤销订单结果
    /// </summary>
    public class ReverseResult : Result
    {
        /// <summary>
        /// 是否需要继续调用撤销，Y-需要，N-不需要
        /// </summary>
        public string recall { get; set; }


    }


    public class Result
    {
        /// <summary>
        /// 微信分配的公众账号ID（付款到银行卡接口，此字段不提供）
        /// </summary>
        public string appid { get; set; }

        /// <summary>
        /// 微信支付分配的商户号
        /// </summary>
        public string mch_id { get; set; }

        #region 服务商
        /// <summary>
        /// 子商户公众账号ID
        /// </summary>
        public string sub_appid { get; set; }

        /// <summary>
        /// 子商户号
        /// </summary>
        public string sub_mch_id { get; set; }

        #endregion

        /// <summary>
        /// 随机字符串，不长于32 位
        /// </summary>
        public string nonce_str { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }

        /// <summary>
        /// SUCCESS/FAIL
        /// </summary>
        public string result_code { get; set; }

        public string err_code { get; set; }
        public string err_code_des { get; set; }

        public string return_code { get; set; }
        public string return_msg { get; set; }
    }


    /// <summary>
    /// 退款异步通知结果
    /// </summary>
    [Serializable, XmlRoot(ElementName = "RefundNotifyResult")]
    public class RefundNotifyResult
    {

        /// <summary>
        /// 公众账号ID
        /// </summary>
        public string appId { get; set; }


        /// <summary>
        /// 退款的商户号
        /// </summary>
        public string mch_id { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string nonce_str { get; set; }

        /// <summary>
        /// 加密信息
        /// </summary>
        public string req_info { get; set; }

        /// <summary>
        /// 返回状态码,SUCCESS/FAIL  此字段是通信标识，非交易标识，交易是否成功需要查看trade_state来判断
        /// </summary>
        public string return_code { get; set; }

        /// <summary>
        /// 返回信息  ，当return_code为FAIL时返回信息为错误原因 ，例如  签名失败、 参数格式校验错误
        /// </summary>
        public string return_msg { get; set; }

        /// <summary>
        /// 返回信息 
        /// </summary>
        public string refund_id { get; set; }

        /// <summary>
        /// 商户退款单号
        /// </summary>
        public string out_refund_no { get; set; }

        /// <summary>
        /// 订单总金额，单位为分，只能为整数
        /// </summary>
        public int total_fee { get; set; }


        /// <summary>
        /// 应结订单金额 ，当该订单有使用非充值券时，返回此字段。应结订单金额=订单金额-非充值代金券金额，应结订单金额<=订单金额。
        /// </summary>
        public int? settlement_total_fee { get; set; }

        /// <summary>
        /// 申请退款金额 ,单位为分 
        /// </summary>
        public int refund_fee { get; set; }

        /// <summary>
        /// 退款金额=申请退款金额-非充值代金券退款金额，退款金额<=申请退款金额 
        /// </summary>
        public int settlement_refund_fee { get; set; }

        /// <summary>
        /// 微信订单号 
        /// </summary>
        public string transaction_id { get; set; }

        /// <summary>
        /// 商户订单号 
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 退款状态 
        /// SUCCESS-退款成功
        ///  CHANGE-退款异常
        ///   REFUNDCLOSE—退款关闭
        /// </summary>
        public string refund_status { get; set; }

        /// <summary>
        /// 资金退款至用户帐号的时间，格式2017-12-15 09:46:01
        /// </summary>
        public DateTime success_time { get; set; }

        /// <summary>
        /// 退款入账账户,取当前退款单的退款入账方 
        /// 退回银行卡： {银行名称}{卡类型}{卡尾号}
        /// 退回支付用户零钱:支付用户零钱
        /// 退还商户: 商户基本账户/商户结算银行账户 
        /// 商户结算银行账户 :支付用户零钱通 
        /// </summary>
        public string refund_recv_accout { get; set; }

        /// <summary>
        /// 退款资金来源
        /// REFUND_SOURCE_RECHARGE_FUNDS 可用余额退款/基本账户
        /// REFUND_SOURCE_UNSETTLED_FUNDS 未结算资金退款
        /// </summary>
        public string refund_account { get; set; }


        /// <summary>
        /// 退款发起来源 
        /// API接口
        /// VENDOR_PLATFORM商户平台
        /// </summary>
        public string refund_request_source { get; set; }
    }


    /// <summary>
    /// 支付异步通知结果
    /// </summary>
    [Serializable, XmlRoot(ElementName = "PayNotifyResult")]
    public partial class PayNotifyResult
    {
        /// <summary>
        /// 返回状态码 SUCCESS/FAIL   此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string return_code
        {
            get; set;
        }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string return_msg
        {
            get; set;
        }


        /// <summary>
        /// 支付账号的appid
        /// </summary>
        public string appid
        {
            get; set;
        }

        /// <summary>
        /// 微信支付分配的商户号
        /// </summary>
        public string mch_id
        {
            get; set;
        }

        /// <summary>
        /// 微信支付分配的终端设备号
        /// </summary>

        public string device_info { get; set; }



        /// <summary>
        /// 随机字符串，不长于32位
        /// </summary>
        public string nonce_str
        {
            get; set;
        }


        /// <summary>
        /// 签名
        /// </summary>
        public string sign
        {
            get; set;
        }

        /// <summary>
        /// 签名类型，目前支持HMAC-SHA256和MD5，默认为MD5
        /// </summary>
        public string sign_type
        {
            get; set;
        }

        /// <summary>
        /// 业务结果 SUCCESS/FAIL
        /// </summary>
        public string result_code
        {
            get; set;
        }

        /// <summary>
        /// 错误代码
        /// </summary>
        public string err_code
        {
            get; set;
        }
        /// <summary>
        /// 错误代码描述
        /// </summary>
        public string err_code_des
        {
            get; set;
        }

        /// <summary>
        /// 随机字符串，不长于32位
        /// </summary>
        public string openid
        {
            get; set;
        }

        /// <summary>
        /// 用户是否关注公众账号，Y-关注，N-未关注
        /// </summary>
        public string is_subscribe
        {
            get; set;
        }


        /// <summary>
        ///交易类型:JSAPI、NATIVE、APP
        /// </summary>
        public string trade_type
        {
            get; set;
        }
        /// <summary>
        /// 付款银行
        /// </summary>
        public string bank_type
        {
            get; set;
        }

        /// <summary>
        /// 订单总金额，单位为分
        /// </summary>
        public int total_fee
        {
            get; set;
        }

        /// <summary>
        /// 应结订单金额=订单金额-非充值代金券金额，应结订单金额<=订单金额
        /// </summary>
        public int? settlement_total_fee { get; set; }

        /// <summary>
        /// 货币类型，符合ISO4217标准的三位字母代码，默认人民币：CNY
        /// </summary>
        public string fee_type
        {
            get; set;
        }

        /// <summary>
        ///  订单现金支付金额
        /// </summary>
        public int cash_fee
        {
            get; set;
        }

        /// <summary>
        /// 货币类型，符合ISO4217标准的三位字母代码，默认人民币：CNY
        /// </summary>
        public string cash_fee_type
        {
            get; set;
        }


        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string transaction_id
        {
            get; set;
        }
        /// <summary>
        /// 商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-|*@ ，且在同一个商户号下唯一。
        /// </summary>
        public string out_trade_no
        {
            get; set;
        }

        /// <summary>
        /// 商家数据包
        /// </summary>
        public string attach
        {
            get; set;
        }

        /// <summary>
        /// 支付完成时间，格式为yyyyMMddHHmmss，如2009年12月25日9点10分10秒表示为20091225091010
        /// </summary>
        public string time_end
        {
            get; set;
        }


    }


}
