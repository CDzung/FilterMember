using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilterMember
{
    public partial class Zalo : Form
    {
        public Zalo()
        {
            InitializeComponent();
        }

        static ChromeDriver cDriver = null;
        private void buttonStart_Click(object sender, EventArgs e)
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--user-data-dir=" + Application.StartupPath + "\\Profiles\\0");

            cDriver = new ChromeDriver(service, options);
            
            cDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            if(textPhoneNumber.Text.Length > 0 || textPassword.Text.Length > 0)
            {
                labelStatus.Text = "login...";
                cDriver.Navigate().GoToUrl("https://id.zalo.me/account?continue=https://chat.zalo.me");
                try
                {
                    cDriver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div[2]/div[1]/ul/li[2]/a")).Click();
                    cDriver.FindElement(By.CssSelector("#input-phone")).SendKeys(textPhoneNumber.Text);
                    
                }
                catch (Exception)
                { 
                }

                try
                {
                    cDriver.FindElement(By.CssSelector("input[type=password]")).SendKeys(textPassword.Text);
                    cDriver.FindElement(By.XPath("//*[@id='app']/div/div/div[2]/div[2]/div[2]/div/div/div/div[4]/a")).Click();
                }
                catch (Exception)
                {
                }
            }

            
            if (cDriver.Url.Contains("https://chat.zalo.me/"))
            {
                labelStatus.Text = "success";
                groupBox.Visible = true;
            }
            else
                labelStatus.Text = "failed";

        }

        private void groupBox_VisibleChanged(object sender, EventArgs e)
        {
            this.Size = new Size(this.Size.Width, this.Visible ? 272 : 160);

        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            var groupList = cDriver.FindElements(By.CssSelector("#conversationList div.conv-item-title__name.truncate > span"));
            foreach (var group in groupList)
            {
                if (group.GetAttribute("innerText").Contains(textGroupName.Text))
                {
                    group.Click();
                    break;
                }
            }

            try
            {
                var pin = cDriver.FindElement(By.CssSelector("#messageView div.content-title-container > span"));
                if (pin.GetAttribute("innerText") == "Bình chọn")
                {
                    cDriver.FindElement(By.CssSelector("#header > div.threadChat.group div.subtitle__groupmember__content.clickable > div")).Click();

                    List<String> memberList = new List<string>();

                    foreach (var member in cDriver.FindElements(By.CssSelector("#member-group div.zl-avatar.chat-box-member__info__avatar.v2.zl-avatar--m > img")))
                    {
                        memberList.Add(member.GetAttribute("src"));
                    }

                    memberList.RemoveAt(0);
                    cDriver.FindElement(By.XPath("/html/body/div/div/div[2]/main/aside/div[2]/div[1]/div/div/div")).Click();
                    pin.Click();

                    cDriver.FindElement(By.CssSelector("#group-poll-create-content > div.group-poll-vote-description.href.underline")).Click();

                    foreach (var member in cDriver.FindElements(By.CssSelector(".group-poll-detail img.zl-avatar__photo, .group-poll-detail zl-avatar__alphabet")))
                    {
                        memberList.Remove(member.GetAttribute("src"));
                    }

                    cDriver.FindElement(By.CssSelector("div.zl-modal__dialog__header i")).Click();
                    cDriver.FindElement(By.CssSelector("div.zl-modal__dialog__header i")).Click();
                    cDriver.FindElement(By.CssSelector("#header > div.threadChat.group div.subtitle__groupmember__content.clickable > div")).Click();

                    IJavaScriptExecutor executor = cDriver as IJavaScriptExecutor;

                    foreach (var member in memberList)
                    {
                        Thread.Sleep(200);
                        try
                        {
                            executor.ExecuteScript("document.querySelector(`#member-group img[src='" + member + "']`).parentElement.parentElement.querySelector(`.fa`).click()");

                        }
                        catch (Exception)
                        {
                            executor.ExecuteScript("document.querySelector(`#member-group img[src='" + member.Substring(6) + "']`).parentElement.parentElement.querySelector(`.fa`).click()");
                        }
                        cDriver.FindElement(By.CssSelector("body > div.popover-v3 > div.zmenu-body > div")).Click();
                        cDriver.FindElement(By.CssSelector("div.zl-modal__footer div.z--btn.z--btn--fill--primary.-lg.--rounded.zl-modal__footer__button > div")).Click();
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }

    

}
