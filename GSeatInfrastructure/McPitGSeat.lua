function LuaExportStart()
	-- Works once just before mission start.
	package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"
	package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"
	socket = require("socket")
	host = host or "127.0.0.1"
	port = port or 8804
	-- c = socket.try(socket.connect(host, port))	--  connect to the listener socket
	-- c:setoption("tcp-nodelay",true)  -- set immediate transmission mode
	udpsend = socket.udp()
	
	JSON = loadfile("Scripts\\JSON.lua")()
	
	payload = {
		Command = "Start"
	}
	socket.try(udpsend:sendto(JSON:encode(payload).." \n", host, port))
end

function LuaExportStop()
	payload = {
		Command = "Stop"
	}
	socket.try(udpsend:sendto(JSON:encode(payload).." \n", host, port))
end

function LuaExportActivityNextEvent(t)
	local ownData = LoGetSelfData();
	local accel = LoGetAccelerationUnits();
	
	payload = 
	{
		Acceleration = {
			X = accel.x,
			Y = accel.y,
			Z = accel.z
		},
		Pitch = ownData.Pitch,
		Roll = ownData.Bank
	}
	
	socket.try(udpsend:sendto(JSON:encode(payload).." \n", host, port))
	
	return t + 0.2	-- This equals the "frame" rate of exports.  Events appear to be each second, so 0.2 = 5 FPS... I think :)
end