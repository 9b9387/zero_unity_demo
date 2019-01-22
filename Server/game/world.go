package game

// World World
type World struct {
	players map[string]*Player
}

var world *World

func init() {
	world = CreateWorld()
}

// CreateWorld 创建世界
func CreateWorld() *World {
	world := &World{
		players: make(map[string]*Player),
	}

	return world
}

// AddPlayer 加入玩家
func (w *World) AddPlayer(p *Player) {
	w.players[p.PlayerID] = p
}

// RemovePlayer 移除玩家
func (w *World) RemovePlayer(id string) {
	if _, ok := w.players[id]; ok {
		delete(w.players, id)
	}
}

// GetPlayer 获取玩家
func (w *World) GetPlayer(id string) *Player {
	if v, ok := w.players[id]; ok {
		return v
	}

	return nil
}

// GetPlayerList 获取全部玩家
func (w *World) GetPlayerList() []*Player {
	list := make([]*Player, 0)
	for _, p := range w.players {
		list = append(list, p)
	}
	return list
}
