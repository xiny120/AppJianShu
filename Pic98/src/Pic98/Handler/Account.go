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
	"strings"

	_ "github.com/go-sql-driver/mysql"
	"github.com/satori/go.uuid"
)

func Account(w http.ResponseWriter, r *http.Request) {

	t, err := template.ParseFiles(
		"wwwroot/tpl/Account.html",
		"wwwroot/tpl/public/header.html",
		"wwwroot/tpl/public/nav.html",
		"wwwroot/tpl/Post.html",
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

// 获取表数据
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
