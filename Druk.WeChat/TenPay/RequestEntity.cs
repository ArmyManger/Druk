using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Druk.WeChat.TenPay
{
    /// <summary>
    /// 生成订单请求对象
    /// </summary>
    [Serializable, XmlRoot(ElementName = "OrderRequest")]
    public class OrderRequest
    {

        /// <summary>
        /// 微信生成的预支付ID，订单费首次支付情况下会有
        /// </summary>
        public string prepayId { get; set; }

        /// <summary>
        /// 商家订单号
        /// </summary>
        public string OutTradeNo { get; set; }
        /// <summary>
        /// 发起的APPID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        ///接收财付通通知的URL （需要在微信后台注册）
        /// </summary>
        public string NotifyUrl { get; set; }
        /// <summary>
        /// 用户的公网ip，不是商户服务器IP
        /// </summary>
        public string CreatIp { get; set; }
        /// <summary>
        /// 商品金额,以分为单位(money * 100).ToString()
        /// </summary>
        public int TotalFee { get; set; }

        /// <summary>
        /// 用户的openId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 收款的商户ID
        /// </summary>
        public string MchId { get; set; }
        /// <summary>
        /// 商品信息
        /// </summary>
        public string Body { get; set; }


        /// <summary>
        ///  附加数据，在查询API和支付通知中原样返回，可作为自定义参数使用。String(127)，如：深圳分店
        /// </summary>
        public string Attach { get; set; }
        /// <summary>
        /// 支付KEY
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 订单生成时间，如果为空，则默认为当前服务器时间 
        /// </summary>
        public DateTime? TimeStart { get; set; }

        /// <summary>   
        /// 订单失效时间，留空则不设置失效时间 
        /// </summary>
        public DateTime? TimeExpire { get; set; }
    }


    /// <summary>
    /// 订单查询/订单撤销 请求对象
    /// </summary>
    public class OrderQueryRequest : OrderCloseRequest
    {

        /// <summary>
        /// 微信订单号(跟OutTradeNo二选一)
        /// </summary>
        public string TransactionId { get; set; }

    }

    /// <summary>
    /// 撤销订单请求对象
    /// </summary>
    public class OrderReverseRequest : OrderQueryRequest
    {

        public OrderReverseRequest()
        {
            TimeOut = 10000;
        }
        /// <summary>
        /// 证书绝对路径，如@"F:\apiclient_cert.p12"
        /// </summary>
        public string Cert { get; set; }

        /// <summary>
        /// 证书密码
        /// </summary>
        public string CertPassword { get; set; }

        /// <summary>
        /// 请求超时设置（以毫秒为单位），默认为10秒。
        /// </summary>
        public int TimeOut { get; set; }


    }

    /// <summary>
    /// 关闭订单请求对象
    /// </summary>
    public class OrderCloseRequest
    {
        public string AppId { get; set; }
        /// <summary>
        /// 收款的商户ID
        /// </summary>
        public string MchId { get; set; }


        /// <summary>
        /// 商家订单号
        /// </summary>
        public string OutTradeNo { get; set; }


        /// <summary>
        /// 支付秘钥
        /// </summary>
        public string Key { get; set; }
    }

    /// <summary>
    /// 申请退款请求
    /// </summary>
    public class OrderRefundRequest : OrderCloseRequest
    {

        /// <summary>
        /// 申请退款请求
        /// </summary>
        public OrderRefundRequest()
        {
            TimeOut = 10000;
            RefundFeeType = "CNY";
            RefundAccount = "REFUND_SOURCE_UNSETTLED_FUNDS";
        }
        /// <summary>
        /// 商户自定义的终端设备号，如门店编号、设备的ID
        /// </summary>
        public string DeviceInfo { get; set; }

        /// <summary>
        /// 微信订单号（和OutTradeNo二选一）
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// 商户侧传给微信的退款单号
        /// </summary>
        public string OutRefundNo { get; set; }

        /// <summary>
        /// 订单金额。订单总金额，单位为分，只能为整数，详见支付金额
        /// </summary>
        public int TotalFee { get; set; }

        /// <summary>
        /// 退款金额。退款总金额，订单总金额，单位为分，只能为整数，详见支付金额
        /// </summary>
        public int RefundFee { get; set; }


        /// <summary>
        ///  货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY，其他值列表详见货币类型
        /// </summary>
        public string RefundFeeType { get; set; }

        /// <summary>
        /// 退款资金来源。仅针对老资金流商户使用
        /// REFUND_SOURCE_UNSETTLED_FUNDS---未结算资金退款（默认使用未结算资金退款）
        ///  REFUND_SOURCE_RECHARGE_FUNDS---可用余额退款(限非当日交易订单的退款）
        /// </summary>
        public string RefundAccount { get; set; }

        /// <summary>
        /// 操作员，操作员帐号, 默认为商户号
        /// </summary>
        public string OpUserId { get; set; }

        /// <summary>
        /// 若商户传入，会在下发给用户的退款消息中体现退款原因
        /// </summary>
        public string RefundDescription { get; set; }

        /// <summary>
        /// 异步接收微信支付退款结果通知的回调地址，通知URL必须为外网可访问的url，不允许带参数。
        /// 如果参数中传了notify_url，则商户平台上配置的回调地址将不会生效。
        /// </summary>
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 证书绝对路径，如@"F:\apiclient_cert.p12"
        /// </summary>
        public string Cert { get; set; }

        /// <summary>
        /// 证书密码
        /// </summary>
        public string CertPassword { get; set; }

        /// <summary>
        /// 请求超时设置（以毫秒为单位），默认为10秒。
        /// </summary>
        public int TimeOut { get; set; }

    }


    /// <summary>
    /// 退款查询请求
    /// </summary>
    public class RefundQueryRequest : OrderCloseRequest
    {
        /// <summary>
        /// 微信订单号（out_trade_no、out_refund_no、refund_id、transaction_id 四选一）
        /// </summary>
        public string TransactionId { get; set; }
        /// <summary>
        /// 商户自定义的终端设备号，如门店编号、设备的ID
        /// </summary>
        public string DeviceInfo { get; set; }

        /// <summary>
        /// 商户侧传给微信的退款单号（out_trade_no、out_refund_no、refund_id、transaction_id 四选一）
        /// </summary>
        public string OutRefundNo { get; set; }

        /// <summary>
        /// 微信生成的退款单号，在申请退款接口有返回（out_trade_no、out_refund_no、refund_id、transaction_id 四选一）
        /// </summary>
        public string RefundId { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        public string SignType { get; set; }
    }
}