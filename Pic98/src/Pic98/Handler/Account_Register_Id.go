package Handler

import (
	"log"
	"net/http"
	"strings"
)

func Id(w http.ResponseWriter, r *http.Request) {
	var result string
	result = "{'status':1,'msg':'Account/Register/Id参数错误！'}"
	param := strings.Split(r.RequestURI, "/")
	if len(param) >= 4 {
		r.ParseForm()
		name := r.Form["name"]

		result = "{'status':0,'msg':'Account/Register/Id调用成功！','data':{'name':'" + name + "'}}"
	} else {
	}
	log.Println(result)
	w.Write([]byte(result))

}
