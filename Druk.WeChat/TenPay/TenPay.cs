using Druk.Common;
using Druk.Common.Helper;
using Senparc.CO2NET.HttpUtility;
using Senparc.Weixin.TenPay.V3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
namespace Druk.WeChat.TenPay
{
    /// <summary>
    /// 微信支付 Senparc.Weixin.TenPay v1.0.0
    /// </summary>
    public class TenPay
    {

        #region 时间戳与随机数
        /// <summary>
        /// 时间戳
        /// </summary>
        public static string TimeStamp
        {
            get
            {
                return TenPayV3Util.GetTimestamp();
            }
        }
        /// <summary>
        /// 随机数
        /// </summary>
        public static string NonceStr
        {
            get
            {
                return TenPayV3Util.GetNoncestr();
            }
        }
        #endregion

        #region 统一下单,返回发起支付对象 
        /// <summary>
        /// 统一下单,返回发起支付对象（将prepayId存储）
        /// </summary>
        /// <param name="dingdan"></param>
        /// <param name="msg">错误消息</param>
        /// <returns></returns>
        public static PaymentResult UnifiedOrder(OrderRequest dingdan, ref string msg)
        {
            msg = "";
            try
            {
                var timeStamp = TimeStamp;
                var nonceStr = NonceStr;
                //实例化对象，构造函数内置了签名
                var xmlDataInfo = new TenPayV3UnifiedorderRequestData(dingdan.AppId, dingdan.MchId, dingdan.Body, dingdan.OutTradeNo,
                     dingdan.TotalFee, dingdan.CreatIp, dingdan.NotifyUrl, Senparc.Weixin.TenPay.TenPayV3Type.JSAPI, dingdan.OpenId, dingdan.Key, nonceStr, null, dingdan.TimeStart, dingdan.TimeExpire, null, dingdan.Attach);

                ////服务商模式，测试子商户号写死了
                //var xmlDataInfoNew=new TenPayV3UnifiedorderRequestData(dingdan.AppId, dingdan.MchId, null, "1626767025", dingdan.Body, dingdan.OutTradeNo, dingdan.TotalFee, dingdan.CreatIp, dingdan.NotifyUrl, Senparc.Weixin.TenPay.TenPayV3Type.JSAPI, dingdan.OpenId,null, dingdan.Key, nonceStr,null, dingdan.TimeStart, dingdan.TimeExpire, null, dingdan.Attach);
                //temp
                Druk.Log.Info(new { action = "支付", xmlDataInfo });

                var result = TenPayV3.Unifiedorder(xmlDataInfo);//调用统一订单接口

                //temp
                Druk.Log.Info(new { action = "支付", xmlDataInfo, result });

                if (result != null && result.result_code == "SUCCESS" && result.return_code == "SUCCESS")
                {
                    var package = "prepay_id=" + result.prepay_id;
                    return new PaymentResult
                    {
                        appId = dingdan.AppId,
                        prepayId = result.prepay_id,
                        nonceStr = nonceStr,
                        package = package,
                        paySign = TenPayV3.GetJsPaySign(dingdan.AppId, timeStamp, nonceStr, package, dingdan.Key),
                        signType = "MD5",
                        timeStamp = timeStamp
                    };
                }
                msg = result.err_code + ":" + result.err_code_des;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return null;


        }
        #endregion

        #region 查询订单
        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static OrderQueryResult QueryOrder(OrderQueryRequest queryInfo)
        {
            if (queryInfo == null)
                return null;

            //实例化查询对象，构造函数内置了签名
            TenPayV3OrderQueryRequestData dataInfo = new TenPayV3OrderQueryRequestData(queryInfo.AppId, queryInfo.MchId, queryInfo.TransactionId, NonceStr, queryInfo.OutTradeNo, queryInfo.Key);

            var result = TenPayV3.OrderQuery(dataInfo);
            if (result != null && result.result_code == "SUCCESS" && result.return_code == "SUCCESS")
                return result.ToJson().ToObjectFromJson<OrderQueryResult>();

            return null;
        }
        #endregion

        #region 关闭订单接口

        /// <summary>
        /// 关闭订单接口,以下情况需要调用关单接口：商户订单支付失败需要生成新单号重新发起支付，要对原订单号调用关单，避免重复支付；系统下单后，用户支付超时，系统退出不再受理，避免用户继续，请调用关单接口。 
        /// 注意：订单生成后不能马上调用关单接口，最短调用时间间隔为5分钟。 
        /// </summary>
        /// <param name="queryInfo"></param>
        /// <returns></returns>
        public static CloseOrderResult CloseOrder(OrderCloseRequest closeInfo, ref string errorMsg)
        {
            if (closeInfo == null)
                return null;

            //实例化查询对象，构造函数内置了签名
            TenPayV3CloseOrderRequestData dataInfo = new TenPayV3CloseOrderRequestData(closeInfo.AppId, closeInfo.MchId, closeInfo.OutTradeNo, closeInfo.Key, NonceStr);

            var result = TenPayV3.CloseOrder(dataInfo);
            errorMsg = result.err_code_des;
            if (result != null && result.result_code == "SUCCESS" && result.return_code == "SUCCESS")
                return result.ToJson().ToObjectFromJson<CloseOrderResult>();


            return null;
        }
        #endregion

        #region 撤销订单,需要证书

        /// <summary>
        /// 撤销订单,需要证书
        /// </summary>
        /// <param name="reverseInfo"></param>
        /// <returns></returns>
        public static ReverseResult ReverseOrder(OrderReverseRequest reverseInfo)
        {
            if (reverseInfo == null)
                return null;

            //实例化查询对象，构造函数内置了签名
            TenPayV3ReverseRequestData dataInfo = new TenPayV3ReverseRequestData(reverseInfo.AppId, reverseInfo.MchId, reverseInfo.TransactionId, NonceStr, reverseInfo.OutTradeNo, reverseInfo.Key);

            var result = TenPayV3.Reverse(dataInfo, reverseInfo.TimeOut);
            if (result != null && result.result_code == "SUCCESS" && result.return_code == "SUCCESS")
                return result.ToJson().ToObjectFromJson<ReverseResult>();

            return null;
        }
        #endregion

        #region 获取支付异步通知结果

        /// <summary>
        /// 获取支付异步通知结果，有对象返回一定是支付成功的
        /// </summary>
        /// <param name="httpContext">NetCore环境必须传入HttpContext实例，不能传Null</param>
        /// <param name="payKey">支付秘钥</param>
        /// <param name="errorMsg">错误消息</param>
        /// <returns></returns>
        public static PayNotifyResult PayNotify(Microsoft.AspNetCore.Http.HttpContext httpContext, string payKey, ref string errorMsg)
        {



            errorMsg = "支付失败";
            PayNotifyResult entity = null;
            try
            {
                ResponseHandler resHandler = new ResponseHandler(httpContext);
                string return_code = resHandler.GetParameter("return_code");
                string return_msg = resHandler.GetParameter("return_msg");

                //设置秘钥
                resHandler.SetKey(payKey);

                //验证请求是否从微信发过来（安全）
                if (resHandler.IsTenpaySign() && return_code.ToUpper() == "SUCCESS")
                {
                    errorMsg = "支付成功";
                    //直到这里，才能认为交易真正成功了，可以进行数据返回
                    entity = new PayNotifyResult
                    {
                        appid = resHandler.GetParameter("appid"),
                        mch_id = resHandler.GetParameter("mch_id"),
                        nonce_str = resHandler.GetParameter("nonce_str"),
                        transaction_id = resHandler.GetParameter("transaction_id"),
                        out_trade_no = resHandler.GetParameter("out_trade_no"),
                        total_fee = resHandler.GetParameter("total_fee").ToInt(),
                        settlement_total_fee = resHandler.GetParameter("settlement_total_fee") != null
                                ? resHandler.GetParameter("settlement_total_fee").ToInt()
                                : null as int?,
                        cash_fee = resHandler.GetParameter("cash_fee").ToInt(),
                        attach = resHandler.GetParameter("attach"),
                        bank_type = resHandler.GetParameter("bank_type"),
                        cash_fee_type = resHandler.GetParameter("cash_fee_type"),
                        device_info = resHandler.GetParameter("device_info"),
                        return_code = return_code,
                        return_msg = return_msg,
                        err_code = resHandler.GetParameter("err_code"),
                        err_code_des = resHandler.GetParameter("err_code_des"),
                        fee_type = resHandler.GetParameter("fee_type"),
                        is_subscribe = resHandler.GetParameter("is_subscribe"),
                        openid = resHandler.GetParameter("openid"),
                        result_code = resHandler.GetParameter("result_code"),
                        sign = resHandler.GetParameter("sign"),
                        sign_type = resHandler.GetParameter("sign_type"),
                        time_end = resHandler.GetParameter("time_end"),
                        trade_type = resHandler.GetParameter("trade_type")
                    };
                    Log.Info(entity);
                }
                else
                {
                    string err_code_des = resHandler.GetParameter("err_code_des");
                    if (!string.IsNullOrEmpty(err_code_des))
                        errorMsg = err_code_des;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                errorMsg = ex.Message;
            }
            return entity;
        }
        #endregion

        #region 退款处理:申请退款》查询退款》处理退款结果


        #region 申请退款，需要证书

        /// <summary>
        /// 申请订单退款，需要证书（保存微信返回的微信退款单号）
        /// </summary>
        /// <param name="refundRequest"></param>
        /// <returns></returns>
        public static RefundResult RefundOrder(OrderRefundRequest refundRequest, ref string errorMsg)
        {
            try
            {
                errorMsg = "退款失败";

                if (refundRequest == null)
                    return null;

                //实例化查询对象，构造函数内置了签名
                TenPayV3RefundRequestData dataInfo = new TenPayV3RefundRequestData(refundRequest.AppId, refundRequest.MchId, refundRequest.Key, refundRequest.DeviceInfo, NonceStr, refundRequest.TransactionId, refundRequest.OutTradeNo, refundRequest.OutRefundNo, refundRequest.TotalFee, refundRequest.RefundFee, refundRequest.OpUserId, refundRequest.RefundAccount, refundRequest.RefundDescription, refundRequest.NotifyUrl, refundRequest.RefundFeeType);

                var result = TenPayV3.Refund(dataInfo, refundRequest.TimeOut);
                errorMsg = result.err_code_des;
                if (result != null && result.result_code == "SUCCESS" && result.return_code == "SUCCESS")
                    return result.ToJson().ToObjectFromJson<RefundResult>();

                return null;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                Log.Error($"申请微信退款失败：{ex.Message}");
                return null;
            }
        }
        #endregion

        #region 退款查询

        /// <summary>
        /// 退款查询 
        /// </summary>
        /// <param name="refundQuery"></param>
        /// <param name="refund_num">查询的第几次的退款信息</param>
        /// <returns></returns>
        public static RefundQueryResult RefundQuery(RefundQueryRequest refundQuery, int refund_num = 1)
        {
            if (refundQuery == null)
                return null;

            //实例化查询对象，构造函数内置了签名
            TenPayV3RefundQueryRequestData dataInfo = new TenPayV3RefundQueryRequestData(refundQuery.AppId, refundQuery.MchId, refundQuery.Key, NonceStr, refundQuery.DeviceInfo, refundQuery.TransactionId, refundQuery.OutTradeNo, refundQuery.OutRefundNo, refundQuery.RefundId);


            #region 手动调用API
            var urlFormat = "https://api.mch.weixin.qq.com/pay/refundquery";
            var data = dataInfo.PackageRequestHandler.ParseXML();
            var formDataBytes = data == null ? new byte[0] : Encoding.UTF8.GetBytes(data);
            MemoryStream ms = new MemoryStream();
            ms.Write(formDataBytes, 0, formDataBytes.Length);
            ms.Seek(0, SeekOrigin.Begin);//设置指针读取位置
            var resultXml = RequestUtility.HttpPost(urlFormat, null, ms);
            var xml = XDocument.Parse(resultXml);

            var result_code = xml.GetXmlValue("result_code");
            if (result_code == "SUCCESS")
            {
                RefundQueryResult model = new RefundQueryResult
                {
                    device_info = xml.GetXmlValue("device_info") ?? "",
                    transaction_id = xml.GetXmlValue("transaction_id") ?? "",
                    out_trade_no = xml.GetXmlValue("out_trade_no") ?? "",
                    total_fee = xml.GetXmlValue("total_fee") ?? "",
                    settlement_total_fee = xml.GetXmlValue("settlement_total_fee") ?? "",
                    fee_type = xml.GetXmlValue("fee_type") ?? "",
                    cash_fee = xml.GetXmlValue("cash_fee") ?? "",
                    refund_count = xml.GetXmlValue("refund_count") ?? "",
                    refund_account = xml.GetXmlValue("refund_account") ?? "",
                    result_code = xml.GetXmlValue("result_code"),
                    err_code = xml.GetXmlValue("err_code"),
                    err_code_des = xml.GetXmlValue("err_code_des"),
                    appid = xml.GetXmlValue("appid"),
                    mch_id = xml.GetXmlValue("mch_id"),
                    nonce_str = xml.GetXmlValue("nonce_str"),
                    sign = xml.GetXmlValue("sign"),
                    refund_status = xml.GetXmlValue($"refund_status_{(refund_num - refund_num)}"),
                    return_code = xml.GetXmlValue("return_code"),
                    return_msg = xml.GetXmlValue("return_msg"),
                    sub_appid = xml.GetXmlValue("sub_appid"),
                    sub_mch_id = xml.GetXmlValue("sub_mch_id"),
                };
                return model;
            }
            #endregion



            #region  //盛派api没返回退款状态，不用他的API

            //var result = TenPayV3.RefundQuery(dataInfo);
            //if (result != null && result.result_code == "SUCCESS" && result.return_code == "SUCCESS")
            //    return result.ToJson().ToObjectFromJson<RefundQueryResult>(); 
            #endregion

            return null;
        }




        #endregion

        #region 退款异步通知结果

        /// <summary>
        /// 获取退款异步通知结果对象，有对象返回一定是支付成功的
        /// </summary>
        /// <param name="httpContext">NetCore环境必须传入HttpContext实例，不能传Null</param>
        /// <param name="payKey">支付秘钥</param>
        /// <param name="errorMsg">错误消息</param>
        /// <returns></returns>
        public static RefundNotifyResult RefundNotify(Microsoft.AspNetCore.Http.HttpContext httpContext, string payKey, ref string errorMsg)
        {


            RefundNotifyResult entity = null;
            try
            {
                ResponseHandler resHandler = new ResponseHandler(httpContext);
                errorMsg = "退款失败";
                if (resHandler != null)
                {
                    string return_code = resHandler.GetParameter("return_code");
                    string return_msg = resHandler.GetParameter("return_msg");
                    errorMsg = return_msg;
                    if (return_code == "SUCCESS")
                    {
                        errorMsg = "退款成功";
                        var req_info = resHandler.GetParameter("req_info");

                        //解密
                        var decodeReqInfo = TenPayV3Util.DecodeRefundReqInfo(req_info, payKey);
                        var decodeDoc = XDocument.Parse(decodeReqInfo);

                        entity = new RefundNotifyResult
                        {
                            appId = resHandler.GetParameter("appid"),
                            mch_id = resHandler.GetParameter("mch_id"),
                            nonce_str = resHandler.GetParameter("nonce_str"),
                            req_info = resHandler.GetParameter("req_info"),
                            transaction_id = decodeDoc.Root.Element("transaction_id").Value,
                            out_trade_no = decodeDoc.Root.Element("out_trade_no").Value,
                            refund_id = decodeDoc.Root.Element("refund_id").Value,
                            out_refund_no = decodeDoc.Root.Element("out_refund_no").Value,
                            total_fee = decodeDoc.Root.Element("total_fee").Value.ToInt(),
                            settlement_total_fee = decodeDoc.Root.Element("settlement_total_fee") != null
                                ? decodeDoc.Root.Element("settlement_total_fee").Value.ToInt()
                                : null as int?,
                            refund_fee = decodeDoc.Root.Element("refund_fee").Value.ToInt(),
                            settlement_refund_fee = decodeDoc.Root.Element("settlement_refund_fee").Value.ToInt(),
                            refund_status = decodeDoc.Root.Element("refund_status").Value,
                            success_time = decodeDoc.Root.Element("success_time").Value.ToDateTime(),
                            refund_recv_accout = decodeDoc.Root.Element("refund_recv_accout").Value,
                            refund_account = decodeDoc.Root.Element("refund_account").Value,
                            refund_request_source = decodeDoc.Root.Element("refund_request_source").Value,
                            return_code = return_code,
                            return_msg = return_msg
                        };

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                errorMsg = ex.Message;
            }
            return entity;
        }
        #endregion

        #endregion

    }
}
