// main.go
package main

import (
	"Pic98/Cfg"
	"Pic98/Handler"
	"log"
	"net/http"
)

func main() {
	Cfg.Cfg["tidb"] = "pic98:vck123456@tcp(106.14.145.51:4000)/Pic98"
	sMux := http.NewServeMux()
	rh := http.RedirectHandler("http://www.baidu.com", 307)
	sMux.Handle("/baidu", rh)
	regidx := http.HandlerFunc(Handler.Index)
	sMux.Handle("/", regidx)
	reghotidol := http.HandlerFunc(Handler.Index_Hotidol)
	sMux.Handle("/Index/Hotidol", reghotidol)
	regnewidol := http.HandlerFunc(Handler.Index_Newidol)
	sMux.Handle("/Index/Newidol", regnewidol)
	regList := http.HandlerFunc(Handler.List)
	sMux.Handle("/List/", regList)
	regDetail := http.HandlerFunc(Handler.Detail)
	sMux.Handle("/Detail/", regDetail)
	rega := http.HandlerFunc(Handler.Account)
	sMux.Handle("/Account/", rega)
	regh := http.HandlerFunc(Handler.Account_Register)
	sMux.Handle("/Account/Register/", regh)
	regid := http.HandlerFunc(Handler.Account_Register_Cmd)
	sMux.Handle("/Account/Register/Cmd", regid)
	regLogin := http.HandlerFunc(Handler.Account_Login)
	sMux.Handle("/Account/Login/", regLogin)
	regi := http.HandlerFunc(Handler.Image)
	sMux.Handle("/Image/", regi)
	regiv := http.HandlerFunc(Handler.Image_Vip)
	sMux.Handle("/Image/Vip/", regiv)
	regibanner := http.HandlerFunc(Handler.Image_Banner)
	sMux.Handle("/Image/Banner/", regibanner)
	regiu := http.HandlerFunc(Handler.Image_Update)
	sMux.Handle("/Image/Update", regiu)

	//regj := http.HandlerFunc(Handler.Js)
	//sMux.Handle("/Js/", regj)
	//regc := http.HandlerFunc(Handler.Css)
	//sMux.Handle("/Css/", regc)
	//sMux.Handle("/css/", regc)
	//regFonts := http.HandlerFunc(Handler.Fonts)
	//sMux.Handle("/Fonts/", regFonts)
	//sMux.Handle("/fonts/", regFonts)
	//regIcon := http.HandlerFunc(Handler.Icon)
	//sMux.Handle("/Icon/", regIcon)
	//sMux.Handle("/icon/", regIcon)

	makeThumbnail := http.HandlerFunc(Handler.MakeThumbnail)
	sMux.Handle("/MakeThumbnail/", makeThumbnail)

	thumbnail := http.HandlerFunc(Handler.Thumbnail)
	sMux.Handle("/thumbnail/", thumbnail)

	php_ueditor_controller := http.HandlerFunc(Handler.Php_ueditor_controller)
	sMux.Handle("/php_ueditor_controller/", php_ueditor_controller)

	log.Println("请用浏览器打开 http://127.0.0.1:3000 ...")
	sMux.Handle("/Js/", http.StripPrefix("/Js/", http.FileServer(http.Dir("wwwroot/js"))))
	sMux.Handle("/Css/", http.StripPrefix("/Css/", http.FileServer(http.Dir("wwwroot/css"))))
	sMux.Handle("/Fonts/", http.StripPrefix("/Fonts/", http.FileServer(http.Dir("wwwroot/fonts"))))
	sMux.Handle("/Icon/", http.StripPrefix("/Icon/", http.FileServer(http.Dir("wwwroot/icon"))))
	sMux.Handle("/3rd/", http.StripPrefix("/3rd/", http.FileServer(http.Dir("wwwroot/3rd"))))
	http.ListenAndServe(":3000", sMux)

}
