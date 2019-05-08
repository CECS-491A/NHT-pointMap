// Deps
const puppeteer = require("puppeteer");
const { expect } = require("chai");

// Constants
const baseURL = process.env.BASE_URL || "localhost:8080/#";
const adminDashURL = `${baseURL}/admindashboard`;

describe("user management", () => {
  let browser;
  before(async () => {
    browser = await puppeteer.launch({
      headless: false,
      args: ["--no-sandbox", "--disable-setuid-sandbox"],
      ignoreDefaultArgs: ["--disable-extensions"]
    });
  });
  beforeall()
  after(async () => {
    await browser.close();
  });

  describe("go to admin dashboard", () => {
    let page;
    before(async () => {
      page = await browser.newPage();
      await page.goto(adminDashURL);
      await page.waitForSelector("main h1");
    });
    after(async () => {
      await page.close();
    });

    it("load user table", async () => {
      await page.waitForSelector("userTable");
      expect("#resetButton").to.be.null;
    });
  });
});
