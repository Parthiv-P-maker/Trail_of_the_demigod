import cv2
import socket

# UDP setup
UDP_IP = "127.0.0.1"
UDP_PORT = 5052
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Face detector
face_cascade = cv2.CascadeClassifier(
    cv2.data.haarcascades + 'haarcascade_frontalface_default.xml'
)

cap = cv2.VideoCapture(0)
frame_width = 640

while True:
    ret, frame = cap.read()
    if not ret:
        break

    frame = cv2.flip(frame, 1)
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    faces = face_cascade.detectMultiScale(gray, 1.3, 5)

    horizontal = 0.0

    for (x, y, w, h) in faces:
        face_center = x + w/2
        center_ratio = face_center / frame_width
        horizontal = (center_ratio - 0.5) * 2
        break

    if abs(horizontal) < 0.12:
        horizontal = 0.0

    horizontal = max(-1, min(1, horizontal))

    sock.sendto(str(horizontal).encode(), (UDP_IP, UDP_PORT))

    cv2.imshow("CV Control", frame)
    if cv2.waitKey(1) & 0xFF == 27:
        break

cap.release()
cv2.destroyAllWindows()