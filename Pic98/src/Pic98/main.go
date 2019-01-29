// main.go
package main

import (
	"Pic98/Cfg"
	"Pic98/Handler"
	"log"
	"math/rand"
	"net/http"
	"time"
)

var HttpMux = http.NewServeMux()

func main() {
	rand.Seed(time.Now().Unix())
	Cfg.Cfg["tidb"] = "pic98:vck123456@tcp(106.14.145.51:4000)/Pic98"

	rh := http.RedirectHandler("http://www.baidu.com", 307)
	HttpMux.Handle("/baidu", rh)
	regidx := http.HandlerFunc(Handler.Index)
	HttpMux.Handle("/", regidx)
	reghotidol := http.HandlerFunc(Handler.Index_Hotidol)
	HttpMux.Handle("/Index/Hotidol", reghotidol)
	regnewidol := http.HandlerFunc(Handler.Index_Newidol)
	HttpMux.Handle("/Index/Newidol", regnewidol)
	regList := http.HandlerFunc(Handler.List)
	HttpMux.Handle("/List/", regList)
	regDetail := http.HandlerFunc(Handler.Detail)
	HttpMux.Handle("/Detail/", regDetail)
	rega := http.HandlerFunc(Handler.Account)
	HttpMux.Handle("/Account/", rega)
	regh := http.HandlerFunc(Handler.Account_Register)
	HttpMux.Handle("/Account/Register/", regh)
	regid := http.HandlerFunc(Handler.Account_Register_Cmd)
	HttpMux.Handle("/Account/Register/Cmd", regid)
	regLogin := http.HandlerFunc(Handler.Account_Login)
	HttpMux.Handle("/Account/Login/", regLogin)
	regPost := http.HandlerFunc(Handler.Account_Post)
	HttpMux.Handle("/Account/Post/", regPost)
	regi := http.HandlerFunc(Handler.Image)
	HttpMux.Handle("/Image/", regi)
	regiv := http.HandlerFunc(Handler.Image_Vip)
	HttpMux.Handle("/Image/Vip/", regiv)
	regibanner := http.HandlerFunc(Handler.Image_Banner)
	HttpMux.Handle("/Image/Banner/", regibanner)
	regiu := http.HandlerFunc(Handler.Image_Update)
	HttpMux.Handle("/Image/Update", regiu)

	makeThumbnail := http.HandlerFunc(Handler.MakeThumbnail)
	HttpMux.Handle("/MakeThumbnail/", makeThumbnail)

	thumbnail := http.HandlerFunc(Handler.Thumbnail)
	HttpMux.Handle("/thumbnail/", thumbnail)

	php_ueditor_controller := http.HandlerFunc(Handler.Php_ueditor_controller)
	HttpMux.Handle("/php_ueditor_controller/", php_ueditor_controller)ß

	log.Println("请用浏览器打开 http://127.0.0.1:3000 ...")
	HttpMux.Handle("/static/", http.StripPrefix("/static/", http.FileServer(http.Dir("wwwroot/static"))))

	http.ListenAndServe(":3000", HttpMux)

}
