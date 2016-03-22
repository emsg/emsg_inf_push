using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Thrift.Protocol;
using Thrift.Transport;
using Thrift;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;

namespace ThriftTest
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public const string host = "push.lcemsg.com";//server address
        public const int port = 4281;
        public const string licence = "8b035d3b57744b669dd8700bf694bc36";//user licence
        public const string sn = "21";

        /**
         * Thrift入口函数:
         * service emsg_inf_push {
         *    string process(1:string licence,2:string sn,3:string content)
         *    string process_batch(1:string licence,2:string sn,3:list<string> contents)
         *}
         *
         * content样例:
         * {
                    "envelope": {
                        "id": "id",
                        "type": 1,
                        "from": "fromuser",
                        "to": "touser",
                        "ack": 1
                    },
                    "vsn": "0.0.1",
                    "payload": {
                        "attrs": {
                            "message_type": "contact",
                            "action": "reject",
                            "contact_icon": "头像",
                            "contact_nickname": "昵称",
                            "contact_id": "好友的用户id"
                        }
                    }
                }
         */
        private void button1_Click(object sender, EventArgs e)
        {
            TTransport transport = null;
            try
            {
                transport = new TSocket(host, port);
                TProtocol protocol = new TBinaryProtocol(transport);
                emsg_inf_push.Client client = new emsg_inf_push.Client(protocol);
                transport.Open();
                string content = createContent();
                var req = client.process(licence, sn, content);
                Console.WriteLine(string.Format("requst : {0}", req));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (null != transport)
                {
                    //close
                    transport.Close();
                }
            }
        }

        //创建发送Json
        private string createContent()
        {
            Envelope en_obj = new Envelope();
            en_obj.id = "testuuid";
            en_obj.type = 1;
            en_obj.from = textBox1.Text;
            en_obj.to = textBox2.Text;
            en_obj.ack = 1;
            Attrs att_obj = new Attrs();
            att_obj.message_type = "contact";
            att_obj.action = "reject";
            att_obj.contact_icon = "test.jpg";
            att_obj.contact_nickname = "testname";
            att_obj.contact_id = "testcontact_id";
            Payload pay_obj = new Payload();
            pay_obj.attrs = att_obj;
            packet pack_obj = new packet();
            pack_obj.envelope = en_obj;
            pack_obj.vsn = "0.0.1";
            pack_obj.payload = pay_obj;
            return stringify(pack_obj);
        }

        public static string stringify(object jsonObject)
        {
            using (var ms = new MemoryStream())
             {
                 new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                 return Encoding.UTF8.GetString(ms.ToArray());
             }
        }

    }

    [DataContract]
    public class Envelope
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public int type { get; set; }

        [DataMember]
        public string from { get; set; }

        [DataMember]
        public string to { get; set; }
        [DataMember]
        public int ack { get; set; }
    }

    [DataContract]
    public class Attrs
    {
        [DataMember]
        public string message_type { get; set; }
        [DataMember]
        public string action { get; set; }
        [DataMember]
        public string contact_icon { get; set; }
        [DataMember]
        public string contact_nickname { get; set; }
        [DataMember]
        public string contact_id { get; set; }
    }

    [DataContract]
    public class Payload
    {
        [DataMember]
        public Attrs attrs { get; set; }
    }

    [DataContract]
    public class packet
    {
        [DataMember]
        public Envelope envelope { get; set; }
        [DataMember]
        public string vsn { get; set; }
        [DataMember]
        public Payload payload { get; set; }
    }
}
