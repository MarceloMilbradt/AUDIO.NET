
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
        res.send(false);

    if (newColor == 'default')
        goToDefault()
    else
        changeColor(newColor);

    prevColor = newColor;
    res.send(true);
})

const goToDefault = async () => {
    await device.find();
    await device.connect();

    await device.set({
        data: {
            '21': 'scene',
            '24': '00000000011a',
        }, multiple: true
    });

    device.disconnect();
}


const changeColor = async (color) => {
    await device.find();
    await device.connect();
    await device.set({
        data: {
            '21': 'colour',
            '24': color,
        }, multiple: true
    });


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
