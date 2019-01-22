package game

import (
	"encoding/json"
	"math/rand"

	"github.com/9b9387/zero"
)

// Player Player
type Player struct {
	PlayerID string        `json:"playerID"`
	X        int           `json:"x"`
	Y        int           `json:"y"`
	Name     string        `json:"name"`
	Type     int           `json:"type"`
	Session  *zero.Session `json:"-"`
}

// CreatePlayer 创建玩家
func CreatePlayer(name string, s *zero.Session) *Player {

	player := &Player{
		PlayerID: name,
		Name:     name,
		X:        rand.Intn(10),
		Y:        rand.Intn(10),
		Type:     rand.Intn(5),
		Session:  s,
	}

	return player
}

// ToJSON 转成json数据
func (p *Player) ToJSON() []byte {
	b, _ := json.Marshal(p)
	return b
}
