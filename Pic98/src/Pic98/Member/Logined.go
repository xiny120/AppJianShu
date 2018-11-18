package Member

import (
	"database/sql"
	"log"

	_ "github.com/go-sql-driver/mysql"
	"github.com/satori/go.uuid"
)

type Userinfo struct {
	Online_key        string
	User_uuid         string
	Un                string
	Idol              bool
	Writer            bool
	Distributor_level int
	Distributor_ref0  int
	Distributor_ref1  int
}

var (

	//sessions := map[string]userinfo{}
	Sessions = make(map[string]Userinfo)
)

// NewV1 returns UUID based on current timestamp and MAC address.
func Login(un string, pwd string) (Userinfo, error) {
	ui := Userinfo{Online_key: ""}
	db, err := sql.Open("mysql", main.Config["tidb"])
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()
	//strsql := fmt.Sprintf("SELECT userguid FROM useridentify where userid='%s'", name)
	stmt, _ := db.Prepare(`SELECT a.userguid FROM Pic98.useridentify a, Pic98.userinfo b 
	where a.userid=? and a.userguid = b.userguid and b.password = ?`)
	log.Println(stmt)
	defer stmt.Close()
	rows, err := stmt.Query(un, pwd)
	log.Println(rows)
	log.Println(err)
	if err == nil {
		defer rows.Close()
		if rows.Next() {
			ok, _ := uuid.NewV4()
			ui.Online_key = ok.String()
			ui.Un = un

			Sessions[ui.Online_key] = ui
		}
	}
	return ui, nil
}
