package Handler

import (
	"Pic98/Cfg"
	"Pic98/Member"
	"html/template"
	"log"
	"net/http"

	"database/sql"
	"encoding/json"
	"fmt"
	"strconv"
	"strings"

	"github.com/PuerkitoBio/goquery"

	_ "github.com/go-sql-driver/mysql"
	"github.com/satori/go.uuid"
)

func Account(w http.ResponseWriter, r *http.Request) {

	t, err := template.ParseFiles(
		"wwwroot/tpl/Account.html",
		"wwwroot/tpl/public/header.html",
		"wwwroot/tpl/public/nav.html",
		"wwwroot/tpl/Post.html",
		"wwwroot/tpl/Article.html",
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

func Account_Register_Cmd(w http.ResponseWriter, r *http.Request) {
	var result string
	result = "{\"status\":1,\"msg\":\"WebApi Account/Register/Cmd 参数错误！\"}"
	param := strings.Split(r.RequestURI, "/")
	if len(param) >= 4 {
		err := r.ParseForm()
		if err != nil {
			result = "{\"status\":1,\"msg\":\"WebApi Account/Register/Cmd ParseForm失败\"}"
		} else {
			cmd := r.FormValue("cmd")
			if cmd != "" {
				db, err := sql.Open("mysql", Cfg.Cfg["tidb"])
				if err != nil {
					log.Fatal(err)
				}
				defer db.Close()
				if cmd == "Login" {
					name := r.FormValue("name")
					pwd := r.FormValue("pwd")

					ui, _ := Account_Register_Cmd_Login(db, name, pwd)
					t00, _ := json.Marshal(ui)
					uidata := string(t00)
					ret := ui.Online_key
					if ret != "" {
						cookie := http.Cookie{Name: "token", Value: ret, Path: "/", MaxAge: 86400 * 10}
						http.SetCookie(w, &cookie)
						log.Println(ui)
						uidata_, _ := json.Marshal(ui)
						uidata = string(uidata_)
					}
					result = fmt.Sprintf("{\"status\":0,\"msg\":\"Account/Register/Id调用成功！\",\"data\":{\"register\":\"%s\",\"ui\":%s}}", ret, uidata)

				} else if cmd == "QueryId" {
					name := r.FormValue("name")

					used, _ := Account_Register_Cmd_CheckId(db, name)
					result = fmt.Sprintf("{\"status\":0,\"msg\":\"Account/Register/Id调用成功！\",\"data\":{\"CheckId\":%d}}", used)
				} else if cmd == "Register" {
					name := r.FormValue("name")
					pwd := r.FormValue("pwd")
					u1, _ := uuid.NewV4()
					uidata := ""
					ui, _ := Account_Register_Cmd_Register(db, name, pwd, u1.String())
					ret := ui.Online_key
					if ret != "" {
						cookie := http.Cookie{Name: "token", Value: ret, Path: "/", MaxAge: 86400 * 10}
						http.SetCookie(w, &cookie)
						log.Println(ui)
						uidata_, _ := json.Marshal(ui)
						uidata = string(uidata_)
					}
					result = fmt.Sprintf("{\"status\":0,\"msg\":\"Account/Register/Id调用成功！\",\"data\":{\"register\":\"%s\",\"ui\":%s}}", ret, uidata)
				} else if cmd == "ModifyPassword" {
					name := r.FormValue("name")
					pwd := r.FormValue("pwd")
					log.Println(name)
					log.Println(pwd)
					ret, _ := Account_Register_Cmd_ModifyPassword(db, name, pwd)

					if ret {
					}
					result = fmt.Sprintf("{\"status\":0,\"msg\":\"Account/Register/ModifyPassword 调用成功！\",\"data\":{\"ModifyPassword\":%t}}", ret)
				}
			} else {
				result = "{\"status\":1,\"msg\":\"WebApi Account/Register/Cmd 参数cmd不能为空！\"}"
			}
		}
	} else {
	}
	log.Println(result)
	w.Write([]byte(result))

}

func Account_Register_Cmd_Login(db *sql.DB, name string, pwd string) (Member.Userinfo, error) {
	ui, _ := Member.Login(name, pwd)
	log.Println(Member.Sessions)
	return ui, nil
}

func Account_Register_Cmd_CheckId(db *sql.DB, name string) (int, error) {
	strsql := fmt.Sprintf("SELECT userguid FROM Pic98.useridentify where userid='%s'", name)
	log.Println(strsql)
	rows, err := db.Query(strsql)
	log.Println(rows)
	log.Println(err)
	if err == nil {
		defer rows.Close()
		if !rows.Next() {
			return 0, nil
		}
	}
	return 1, err
}

func Account_Register_Cmd_Register(db *sql.DB, name string, pwd string, userguid string) (Member.Userinfo, error) {
	stmt, _ := db.Prepare("INSERT INTO userinfo(userguid,password,nick_name) VALUES (?,?,?)")
	log.Println(stmt)
	defer stmt.Close()
	_, err := stmt.Exec(userguid, pwd, name)
	if err != nil {
		fmt.Printf("insert data error: %v\n", err)
		return Member.Userinfo{Online_key: ""}, nil
	}

	stmt, _ = db.Prepare("INSERT INTO useridentify (userguid,userid) VALUES(?,?)")
	defer stmt.Close()
	_, err = stmt.Exec(userguid, name)
	if err != nil {
		fmt.Printf("insert data error: %v\n", err)
		return Member.Userinfo{Online_key: ""}, nil
	}

	ui, _ := Member.Login(name, pwd)
	log.Println(Member.Sessions)

	return ui, nil
}

func Account_Register_Cmd_ModifyPassword(db *sql.DB, name string, pwd string) (bool, error) {
	stmt, _ := db.Prepare("update userinfo  left join useridentify  on userinfo.userguid = useridentify.userguid set userinfo.password = ? where useridentify.userid=?")
	log.Println(stmt)
	defer stmt.Close()
	_, err := stmt.Exec(pwd, name)
	if err != nil {
		fmt.Printf("insert data error: %v\n", err)
		return false, err
	}

	return true, nil
}

func Account_Login(w http.ResponseWriter, r *http.Request) {
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

func Account_Register(w http.ResponseWriter, r *http.Request) {
	//fmt.Fprintf(w, "%s", "register now!")
	t, err := template.ParseFiles(
		"wwwroot/tpl/Register.html",
		"wwwroot/tpl/public/header.html",
		"wwwroot/tpl/public/nav.html",

		"wwwroot/tpl/public/footer.html")
	if err != nil {
		log.Fatal(err)
	}

	data := struct {
		Title string
	}{
		Title: "注册新用户",
	}

	err = t.Execute(w, data)
	if err != nil {
		log.Fatal(err)
	}
}

func Account_Post(w http.ResponseWriter, r *http.Request) {
	//editorValue title
	result := "{\"status\":1,\"msg\":\"WebApi Account/Register/Cmd ParseForm失败\"}"
	err := r.ParseForm()
	if err != nil {
		//result := "{\"status\":1,\"msg\":\"WebApi Account/Register/Cmd ParseForm失败\"}"
	} else {
		//for k, v := range r.Form {
		//	fmt.Printf("key: %v\n", k)
		//	fmt.Printf("val: %v\n", strings.Join(v, ";"))
		//}
		Title := r.FormValue("title")
		HotLabelText := r.FormValue("hotlabeltext")
		Idol_type, _ := strconv.Atoi(r.FormValue("idol-type"))
		Idol_name := "00000000-0000-0000-0000-000000000000"
		if Idol_type > 0 {
			Idol_name = r.FormValue("idol-name")
		}
		Content := r.FormValue("editorValue")
		aguid, aguide := uuid.NewV4()
		if Title == "" || HotLabelText == "" || Idol_name == "" || Content == "" || aguide != nil {
			result = "{\"status\":1,\"msg\":\"WebApi Account/Post 标题，标签，模特，内容，uuid  失败\"}"
		} else {
			log.Println(aguid)
			// Load the HTML document
			doc, err := goquery.NewDocumentFromReader(strings.NewReader(Content))
			if err != nil {
				log.Fatal(err)
			}

			// Find the review items
			doc.Find("img").Each(func(i int, s *goquery.Selection) {
				// For each item fou
				log.Println(s.Attr("src"))
			})

		}

	}

	fmt.Fprintf(w, "%s", result)
}

func Account_Post_Param(w http.ResponseWriter, r *http.Request) {
	type tag struct {
		Aguid string
		Label string
	}

	tags := []tag{}

	db, err := sql.Open("mysql", Cfg.Cfg["tidb"])
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()
	stmt, _ := db.Prepare(`SELECT aguid,label FROM Pic98.tags`)
	log.Println(stmt)
	defer stmt.Close()
	rows, err := stmt.Query()
	log.Println(rows)
	log.Println(err)
	if err == nil {
		defer rows.Close()
		for rows.Next() {
			tag0 := tag{Aguid: "", Label: ""}
			rows.Scan(&tag0.Aguid, &tag0.Label)
			tags = append(tags, tag0)
		}
	}
	tags_, _ := json.Marshal(tags)
	result := string(tags_)
	log.Println(result)
	w.Write([]byte(result))
}

func Account_Post_Param_Idol(w http.ResponseWriter, r *http.Request) {
	type tag struct {
		Userguid  string
		Nick_name string
	}

	tags := []tag{}

	db, err := sql.Open("mysql", Cfg.Cfg["tidb"])
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()
	stmt, _ := db.Prepare(`SELECT userguid,nick_name FROM Pic98.userinfo where idol > 0`)
	log.Println(stmt)
	defer stmt.Close()
	rows, err := stmt.Query()
	log.Println(rows)
	log.Println(err)
	if err == nil {
		defer rows.Close()
		for rows.Next() {
			tag0 := tag{Userguid: "", Nick_name: ""}
			rows.Scan(&tag0.Userguid, &tag0.Nick_name)
			tags = append(tags, tag0)
		}
	}
	tags_, _ := json.Marshal(tags)
	result := string(tags_)
	log.Println(result)
	w.Write([]byte(result))
}
