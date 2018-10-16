package Handler

import (
	//"encoding/json"
	"database/sql"
	"fmt"
	"log"
	"net/http"
	"strings"

	"Pic98/Member"

	_ "github.com/go-sql-driver/mysql"
	"github.com/satori/go.uuid"
)

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
				db, err := sql.Open("mysql", "pic98:vck123456@tcp(106.14.145.51:4000)/Pic98")
				if err != nil {
					log.Fatal(err)
				}
				defer db.Close()
				if cmd == "QueryId" {
					name := r.FormValue("name")

					used, _ := Account_Register_Cmd_CheckId(db, name)
					result = fmt.Sprintf("{\"status\":0,\"msg\":\"Account/Register/Id调用成功！\",\"data\":{\"CheckId\":%d}}", used)
				} else if cmd == "Register" {
					name := r.FormValue("name")
					pwd := r.FormValue("pwd")
					u1, _ := uuid.NewV4()
					ret, _ := Account_Register_Cmd_Register(db, name, pwd, u1.String())
					result = fmt.Sprintf("{\"status\":0,\"msg\":\"Account/Register/Id调用成功！\",\"data\":{\"register\":\"%s\"}}", ret)
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

func Account_Register_Cmd_Register(db *sql.DB, name string, pwd string, userguid string) (string, error) {
	stmt, _ := db.Prepare("INSERT INTO userinfo(userguid,password,nick_name) VALUES (?,?,?)")
	log.Println(stmt)
	defer stmt.Close()
	_, err := stmt.Exec(userguid, pwd, name)
	if err != nil {
		fmt.Printf("insert data error: %v\n", err)
		return "", nil
	}

	stmt, _ = db.Prepare("INSERT INTO useridentify (userguid,userid) VALUES(?,?)")
	defer stmt.Close()
	_, err = stmt.Exec(userguid, name)
	if err != nil {
		fmt.Printf("insert data error: %v\n", err)
		return "", nil
	}

	online_key, _ := Member.Login(name, pwd)

	return online_key, nil
}
