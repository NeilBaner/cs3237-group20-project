# Serve model as a flask application

import pickle as pk
import numpy as np
from flask import Flask, request
from collections import deque

model = None
app = Flask(__name__)

filename = 'final_fall_detection_model.sav'

def load_model():
    global model
    model = pk.load(open(filename, 'rb'))


@app.route('/')
def home_endpoint():
    return 'Hello World!'


@app.route('/predict', methods=['POST'])
def get_prediction():
    # Works only for a single sample
    if request.method == 'POST':
        
        data = request.get_json()  # Get data posted as a json
        print(data)
        data = np.array(data["data"])
        prediction = model.predict(data)
        return str(prediction)


if __name__ == '__main__':
    load_model()  # load model at the beginning once only
    app.run(host='0.0.0.0', port=80)
    