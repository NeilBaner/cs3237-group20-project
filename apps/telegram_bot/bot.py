import os
import logging
import telegram
from telegram.ext import Updater, CommandHandler, MessageHandler, Filters
import pyodbc
from credentials import URL, bot_token, driver, server, database, username, password

## Resources:
# https://towardsdatascience.com/how-to-deploy-a-telegram-bot-using-heroku-for-free-9436f89575d2
# https://github.com/liuhh02/python-telegram-bot-heroku

bot = telegram.Bot(token=bot_token)
PORT = int(os.environ.get('PORT', 8443))

# Enable logging
logging.basicConfig(format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
                    level=logging.INFO)

logger = logging.getLogger(__name__)
exercise = []

def queryDb() :
    print("Querying database")
    with pyodbc.connect('DRIVER='+driver+';SERVER=tcp:'+server+';PORT=1433;DATABASE='+database+';UID='+username+';PWD='+ password) as conn:
        with conn.cursor() as cursor:
            ## This is the query to get all columns from the table
            ## These are all the columns we were working with before the IMUs died
            cursor.execute("SELECT SUM(bicep_curl), SUM(shoulder_press), SUM(left_shoulder_lateral_raise), SUM(right_shoulder_lateral_raise), SUM(shoulder_front_raise), SUM(left_hand_tricep_extension), SUM(right_hand_tricep_extension), SUM(forward_lunge), SUM(dumbbell_squat) from dbo.exercise_table")
            row = cursor.fetchone()
            while row:
                print (str(row[0]) + " " + str(row[1]))
                for col in row: 
                    print(col)
                    exercise.append(col)
                row = cursor.fetchone()  
    print(exercise)

def start(update, context):
    update.message.reply_text(
        "Hello I'm Boyd. I'm here to help you monitor the health of your loved one. How may I help you? You can start with `/help` to see the list of available commands.")

def help(update, context):
    update.message.reply_text(
        """
        Here is the list of available commands:
        /checkin - check the status of your loved one
        /exercise - check if your loved one has exercised today
        /exercisehistory - check the full exercise history of your loved one
        /help - show the list of available commands
        """)

def unknown_text(update, context):
    update.message.reply_text(
        "Sorry there is no such command. You said '%s'" % update.message.text)

def checkin(update, context):
    queryDb()
    if (sum(exercise) > 0):
        hasExercised = "Yes"
    else:
        hasExercised = "No" 
    update.message.reply_text(
        f"""
        Here is the status:
            Exercised: {hasExercised}
            Total Duration of Exercise (in seconds): {exercise[0]+exercise[1]}
                Bicep Curl: {exercise[0]}
                Squat: {exercise[1]}
        """)

def exerciseDur(update, context):
    queryDb()
    if (sum(exercise) > 0):
        hasExercised = "Yes"
    else:
        hasExercised = "No" 
    update.message.reply_text(
        f"""
        Here is the exercise status for today:
            Exercised: {hasExercised}
            Total Duration of Exercise: {exercise[0]+exercise[1]}
        """)

def exercisehistory (update, context):
    queryDb()
    update.message.reply_text(
        f"""
        Here is the full exercise history (in seconds) for today:
            Bicep curl: {exercise[0]}
            Squat: {exercise[1]}
        """)

def echo(update, context):
    """Echo the user message."""
    update.message.reply_text(update.message.text)

def error(update, context):
    """Log Errors caused by Updates."""
    logger.warning('Update "%s" caused error "%s"', update, context.error)

def main():
    """Start the bot."""
    # Create the Updater and pass it your bot's token.
    # Make sure to set use_context=True to use the new context based callbacks
    # Post version 12 this will no longer be necessary
    updater = Updater(bot_token, use_context=True)

    # Get the dispatcher to register handlers
    dp = updater.dispatcher

    # on different commands - answer in Telegram
    dp.add_handler(CommandHandler("start", start))
    dp.add_handler(CommandHandler("help", help))
    dp.add_handler(CommandHandler("checkin", checkin))
    dp.add_handler(CommandHandler("exercise", exerciseDur))
    dp.add_handler(CommandHandler("exercisehistory", exercisehistory))


    # on noncommand i.e message - echo the message on Telegram
    dp.add_handler(MessageHandler(Filters.text, echo))

    # log all errors
    dp.add_error_handler(error)

    ## The code below is needed to run the bot on Heroku (did not use this during the demo)
    # Start the Bot
    """   
    updater.start_webhook(
        listen="0.0.0.0",
        port=int(PORT),
        url_path=bot_token,
        webhook_url=URL + bot_token
    )

    updater.idle()
    """

    ## The code below is needed to run the telegram bot locally
    # Start the Bot
    updater.start_polling()

    # Run the bot until you press Ctrl-C or the process receives SIGINT,
    # SIGTERM or SIGABRT. This should be used most of the time, since
    # start_polling() is non-blocking and will stop the bot gracefully.
    updater.idle()

    # Run the bot until you press Ctrl-C or the process receives SIGINT,
    # SIGTERM or SIGABRT. This should be used most of the time, since
    # start_polling() is non-blocking and will stop the bot gracefully.
    updater.idle()

if __name__ == '__main__':
    main()