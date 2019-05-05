// Deps
const puppeteer = require('puppeteer');

// Constants
const basePointMapURL = "localhost:8080/#";
const mapViewURL = `${basePointMapURL}/mapview`;
const existingPoint = "";
const pointDetailsURL = `${basePointMapURL}/pointdetails` + existingPoint;
const TYPE_DELAY = 0;

let loadingTimeout = () => new Promise(resolve => setTimeout(resolve, 2000))
let mapCenterTimeout = () => new Promise(resolve => setTimeout(resolve, 2000))
let viewTimeout = () => new Promise(resolve => setTimeout(resolve, 15000))

let fillPointEditor = async page => {
    await page.type('#name', 'pointName', {delay: TYPE_DELAY});
    await page.type('#description', 'pointDesc', {delay: TYPE_DELAY});
    await page.type('#longitude', '-118.1103291607132');
    await page.type('#latitude', '33.782872949141556');
    await page.click('#saveButton');
}

let fillPointEditorChange = async page => {
    await page.type('#name', (await page.element('#name').textContent) + "asdf", {delay: TYPE_DELAY});
    await page.type('#description', (await page.element('#description').textContent) + "asdf", {delay: TYPE_DELAY});
    await page.type('#longitude', '-118.1103291607132');
    await page.type('#latitude', '33.782872949141556');
    await page.click('#saveButton');
}

let getPointMapLaunch = async page => {
    var launchElements = await page.evaluate(() => page.document.getElementsByClassName('headline mb-0'));
    console.log(launchElements);
    return launchElements.forEach(element => {
        console.log(element.innerHTML.textContent);
        if(element.innerHTML.textContent === "Pointmap") {
            return element.innerHTML;
        }
    });
}

(async () => {
    let browser = await puppeteer.launch({ headless: false });

    let page = await browser.newPage();
    await page.goto(mapViewURL);
    await loadingTimeout();
    //fills out point editor page
    await page.click('#addPointBtn');
    await mapCenterTimeout();
    //logs in
    await fillPointEditor(page);
    await page.click('#saveButton');
    //redirects to map view
    await loadingTimeout();
    await viewTimeout();

    page = await browser.newPage();

    await page.goto(pointDetailsURL);
    await loadingTimeout();

    await page.click('#editButton');
    await loadingTimeout();
    await fillPointEditorChange();
    await viewTimeout();

    await page.close();
    await browser.close();
})();
