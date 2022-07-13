const TuyAPI = require('tuyapi');
const express = require('express');
const fs = require('fs');
const config = JSON.parse(fs.readFileSync('device.json', 'utf8'));
const { devices, defaults}= config;
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
    await devices.forEach(async conf => {
        const device = new TuyAPI(conf);
        console.log("Reset Colour of device");
        await device.find();
        await device.connect();
        await device.set({ dps: '21', set: defaults.mode });
        await device.set({ dps: '24', set: defaults.value });

        device.disconnect();
    });
}


const changeColor = async (color) => {
    await devices.forEach(async conf => {
        const device = new TuyAPI(conf);
        console.log("Trying to set Colour to: " + color);
        await device.find();
        await device.connect();
        await device.set({ dps: '21', set: 'colour' });
        await device.set({ dps: '24', set: color });
        device.disconnect();
    });
}


app.listen(port, () => {
    console.log(`Example app listening on port ${port}`)
})


const device = new TuyAPI(devices[0]);



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
