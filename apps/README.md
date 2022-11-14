# Telegram Bot (@boydappbot)
Intially running on heroku, but it could not work well with the Azure Database. I did not include the credentials in the folder, but `credentials.py` is a python file containing the necessary information to run the bot (database credentials, telegram bot api token, heroku url) 

1. Download the libraries from requirements.txt
2. Run the file with `python bot.py`

:exclamation: This will not run as intended because I did not include the credentials file.

## Resources
- [python telegram bot library](https://pypi.org/project/python-telegram-bot/)
- [code from github](https://github.com/liuhh02/python-telegram-bot-heroku)

### API endpoint for fall detection
this sends the message to me @franceneee
- `https://api.telegram.org/bot5767852164:AAGjjY1I5_mUF4k-NAjeGrjV8irYvC1nPAQ/sendMessage?chat_id=615729062&text=EMERGENCY!! Your loved one has fallen. <b>Please</b> check on them immediately.`


# PWA website
This PWA website allows the website to be added onto mobile devices like an app from the app store. 

1. Run `npm install` to get all required node modules
2. Then `npm start` to see the app

Otherwise, see the hosted app [here](https://boyd-app.vercel.app/).

## Resources 
- [PWA with React](https://create-react-app.dev/docs/making-a-progressive-web-app/)
