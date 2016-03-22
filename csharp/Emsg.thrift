namespace java com.cc14514.inf

/**
*入口函数
*/
service emsg_inf_push {
    string process(1:string licence,2:string sn,3:string content)
    string process_batch(1:string licence,2:string sn,3:list<string> contents)
}