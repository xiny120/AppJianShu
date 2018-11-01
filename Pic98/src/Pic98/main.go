// main.go
package main

import (
	"log"
	"net/http"
)
import (
	"Pic98/Handler"
)

func main() {

	sMux := http.NewServeMux()
	rh := http.RedirectHandler("http://www.baidu.com", 307)
	sMux.Handle("/baidu", rh)
	regidx := http.HandlerFunc(Handler.Index)
	sMux.Handle("/", regidx)
	regList := http.HandlerFunc(Handler.List)
	sMux.Handle("/List/", regList)
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
	regj := http.HandlerFunc(Handler.Js)
	sMux.Handle("/Js/", regj)
	regc := http.HandlerFunc(Handler.Css)
	sMux.Handle("/Css/", regc)
	sMux.Handle("/css/", regc)
	regFonts := http.HandlerFunc(Handler.Fonts)
	sMux.Handle("/Fonts/", regFonts)
	sMux.Handle("/fonts/", regFonts)
	regIcon := http.HandlerFunc(Handler.Icon)
	sMux.Handle("/Icon/", regIcon)
	sMux.Handle("/icon/", regIcon)

	log.Println("Listening...")
	http.ListenAndServe(":3000", sMux)

}
