# Serve model as a flask application

import pickle
import json
import numpy as np
from flask import Flask, request
from tensorflow import keras
from collections import deque

from sklearn import preprocessing
# from sklearn.preprocessing import StandardScaler

model = None
app = Flask(__name__)

window = 50
slide = 10
data_q = deque(maxlen=window)

open('log.txt', 'w').close()

def load_model():
    global model
    model = keras.models.load_model('lstm.hd5')


@app.route('/')
def home_endpoint():
    return 'Hello World!'


@app.route('/predict', methods=['POST'])
def get_prediction():
    if request.method == 'POST':
        print(f"request data: {request.data}")

        data = request.get_json()  # Get data posted as a json
        
        print(f"data: {data}")
        

        data = np.array(data["data"])
        for d in data:
            with open('log.txt', 'a') as f:
                f.write(f"{d}\n")
            data_q.append(d)
        if len(data_q) == window:
            data = np.asarray(data_q)
            scaler = preprocessing.StandardScaler().fit(data)
            data = scaler.transform(data)
            prediction = np.argmax(model.predict(np.expand_dims(data, axis=0), verbose = 0))
            print(f"prediction: {str(prediction)}")
            return str(prediction)
        else:
            return("insufficient data")


if __name__ == '__main__':
    load_model()  # load model at the beginning once only
    app.run(host='0.0.0.0', port=80)