const int trigPin1 = 2;  // Trig pin za prvi parking
const int echoPin1 = 3;  // Echo pin za prvi parking
const int trigPin2 = 5; // Trig pin za drugi parking
const int echoPin2 = 4; // Echo pin za drugi parking

const int PARKING_THRESHOLD = 20; // Udaljenost u cm ispod koje se parking smatra zauzetim

void setup() {
  Serial.begin(9600); // Inicijalizacija serijske komunikacije
  
  // Postavljanje pinova
  pinMode(trigPin1, OUTPUT);
  pinMode(echoPin1, INPUT);
  pinMode(trigPin2, OUTPUT);
  pinMode(echoPin2, INPUT);
}

void loop() {
  // Provera statusa za oba parking mesta
  bool parking1Status = checkParkingStatus(trigPin1, echoPin1);
  bool parking2Status = checkParkingStatus(trigPin2, echoPin2);
  
  // Slanje statusa preko serijske komunikacije
  Serial.print(parking1Status ? "1:SLOBODAN," : "1:ZAUZET,");
  Serial.println(parking2Status ? "2:SLOBODAN" : "2:ZAUZET");
  
  delay(1000); // Provera svake sekunde
}

bool checkParkingStatus(int trigPin, int echoPin) {
  // Slanje ultrazvučnog pulsa
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  
  // Merenje trajanja povratnog signala
  long duration = pulseIn(echoPin, HIGH);
  
  //Računjanje udaljenosti
  int distance = duration * 0.034 / 2;
  
  // Vraća true ako je parking slobodan (iznad threshold-a)
  return distance > PARKING_THRESHOLD;
}