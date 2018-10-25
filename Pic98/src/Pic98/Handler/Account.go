package Handler

import (
	"html/template"
	"log"
	"net/http"
)

func Account(w http.ResponseWriter, r *http.Request) {

	t, err := template.ParseFiles(
		"wwwroot/tpl/Account.html",
		"wwwroot/tpl/public/header.html",
		"wwwroot/tpl/public/nav.html",
		"wwwroot/tpl/public/footer.html")
	if err != nil {
		log.Fatal(err)
	}

	data := struct {
		Title string
	}{
		Title: "用户中心",
	}

	err = t.Execute(w, data)
	if err != nil {
		log.Fatal(err)
	}
}

func Account_Setup(w http.ResponseWriter, r *http.Request) {

	t, err := template.ParseFiles(
		"wwwroot/tpl/Account_Setup.html",
		"wwwroot/tpl/public/header.html",
		"wwwroot/tpl/public/nav_m_ucenter.html",
		"wwwroot/tpl/public/footer.html")
	if err != nil {
		log.Fatal(err)
	}

	data := struct {
		Title string
	}{
		Title: "用户中心",
	}

	err = t.Execute(w, data)
	if err != nil {
		log.Fatal(err)
	}
}
