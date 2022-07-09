
const TuyAPI = require('tuyapi');
const express = require('express');
const fs = require('fs');
const config = JSON.parse(fs.readFileSync('device.json', 'utf8'));

const app = express()
const port = 3000

let prevColor = "";

app.get('/:color', (req, res) => {
    let newColor = req.params.color;

    if (newColor === prevColor)
        return res.send(false);

    if (newColor == 'default')
        goToDefault()
    else
        changeColor(newColor);

   prevColor = newColor;
   return  res.send(true);
})

const goToDefault = async () => {
    const device = new TuyAPI(config);
    console.log("Reset Colour of device");
    await device.find();
    await device.connect();
    await device.set({ dps: '21', set: 'scene' });
    await device.set({ dps: '24', set: "00000000011a" });

    device.disconnect();
}


const changeColor = async (color) => {
    const device = new TuyAPI(config);
    console.log("Trying to set Colour to: " + color);
    await device.find();
    await device.connect();
    await device.set({ dps: '21', set: 'colour' });
    await device.set({ dps: '24', set: color });
    device.disconnect();
}


app.listen(port, () => {
    console.log(`Example app listening on port ${port}`)
})


const device = new TuyAPI(config);



device.find().then(() => {
    device.connect();
    device.disconnect();
});

device.on('connected', () => {
    console.log('Connected to device!');
});

device.on('disconnected', () => {
    console.log('Disconnected from device.');
});

device.on('error', error => {
    console.log('Error!', error);
});
