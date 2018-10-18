package Handler

import (
	"html/template"
	"log"
	"net/http"

	"Pic98/Member"
)

func Login(w http.ResponseWriter, r *http.Request) {
	//fmt.Fprintf(w, "%s", "register now!")
	t, err := template.ParseFiles(
		"wwwroot/tpl/Login.html",
		"wwwroot/tpl/public/header.html",
		"wwwroot/tpl/public/nav.html",
		"wwwroot/tpl/public/footer.html")
	if err != nil {
		log.Fatal(err)
	}

	err = r.ParseForm()
	if err != nil {
		//result := "{\"status\":1,\"msg\":\"WebApi Account/Register/Cmd ParseForm失败\"}"
	} else {
		cmd := r.FormValue("cmd")
		if cmd == "out" {
			cookie, err := r.Cookie("token")
			if err == nil {
				cookievalue := cookie.Value
				delete(Member.Sessions, cookievalue)
				cookie := http.Cookie{Name: "token", Path: "/", MaxAge: -1}
				http.SetCookie(w, &cookie)
			} else {

			}

		}
	}

	data := struct {
		Title string
	}{
		Title: "用户登陆",
	}

	err = t.Execute(w, data)
	if err != nil {
		log.Fatal(err)
	}
}
