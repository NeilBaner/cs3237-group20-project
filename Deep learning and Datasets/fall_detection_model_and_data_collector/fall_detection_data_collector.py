from flask import Flask, request, render_template, Response
import csv
import time 

app = Flask(__name__)

# Create just a single route to read data from our WeMOS
@app.route('/data', methods = ['GET'])
def addData():
    ''' The one and only route. It extracts the
    data from the request, converts to float if the
    data is not None, then calls the callback if it is set
    '''
    global _callback_

    gyro_x_str = request.args.get('gyro_x')
    gyro_y_str = request.args.get('gyro_y')
    gyro_z_str = request.args.get('gyro_z')
    accel_x_str = request.args.get('accel_x')
    accel_y_str = request.args.get('accel_y')
    accel_z_str = request.args.get('accel_z')
    resultant_g_str = request.args.get('resultantG')

    #Gyroscope Values
    gyro_x = float(gyro_x_str) if gyro_x_str else None
    gyro_y = float(gyro_y_str) if gyro_y_str else None
    gyro_z = float(gyro_z_str) if gyro_z_str else None
    
    #Accelorometer Values
    accel_x = float(accel_x_str) if accel_x_str else None
    accel_y = float(accel_y_str) if accel_y_str else None
    accel_z = float(accel_z_str) if accel_z_str else None     
    resultant_g = float(resultant_g_str) if resultant_g_str else None
    label = 'Normal'
    data_record = [time.time(), gyro_x, gyro_y, gyro_z, accel_x, accel_y, accel_z, resultant_g, label]

    print("Gyroscope Data: x: {}, y: {}, z: {}\n".format(gyro_x, gyro_y, gyro_z))
    print('Accelorometer Data x: {}, y: {}, z: {}, resultant_g: {}\n'.format(accel_x, accel_y, accel_z, resultant_g))
    
    with open('fall_detection_data.csv', 'a', encoding='UTF8') as f:
        writer = csv.writer(f)
        # write the data
        writer.writerow(data_record)

    return "OK", 200

def main():
    app.run(host = "0.0.0.0", port = '3237')


if __name__ == '__main__':
    main()

