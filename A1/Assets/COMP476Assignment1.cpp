/******************************************************************************

                            COMP476 Assignment 1

*******************************************************************************/

#include <iostream>
#include <cmath>

using namespace std;
//Question 1 starts
float original_velocity[] = {3, 1};
float original_position[] = {5, 6};
float target_original_position[] = {8, 2};
float maxium_acceleration = 15;
float time_slot = 0.4;
float maxium_velocity_magnitude = 5;

//checked
float * Distance(float target[], float self[])
{
    float * distance = new float[2];
    distance[0] = target[0] - self[0];
    distance[1] = target[1] - self[1];
    float distance_magenitude = sqrt(pow(distance[0], 2) + pow(distance[1], 2));
    distance[0] = distance[0] / distance_magenitude;
    distance[1] = distance[1] / distance_magenitude;
    //cout << "distance_magenitude = " << distance_magenitude << endl;
    return  distance;
};

//checked
float OriginalVelocityMagnitude()
{
    return sqrt(pow(original_velocity[0], 2) + pow(original_velocity[1], 2));   
};

float * NextAcc(float target[], float self[])
{
    float * distance = Distance(target, self);
    float * acc = new float[2];
    acc[0] = maxium_acceleration * distance[0];
    acc[1] = maxium_acceleration * distance[1];
    cout << "distance = (" << distance[0] << ", " << distance[2] << ")\n";
    delete distance;
    return acc;
};

float * NewVelocitySteering(float target[], float self[], float velocity[], float time_slot)
{
    float * acceleration = NextAcc(target, self);
    float * new_velocity = new float[2];
    new_velocity[0] = velocity[0] + acceleration[0] * time_slot;
    new_velocity[1] = velocity[1] + acceleration[1] * time_slot;
    float velocity_magnitude = sqrt(pow(new_velocity[0], 2) + pow(new_velocity[1], 2));
    if (velocity_magnitude > maxium_velocity_magnitude)
    {
        new_velocity[0] = maxium_velocity_magnitude * new_velocity[0] / velocity_magnitude;
        new_velocity[1] = maxium_velocity_magnitude * new_velocity[1] / velocity_magnitude;
    }
    cout << "new acceleration = (" << acceleration[0] << ", " << acceleration[1] << ")\n";
    cout << "new velocity = (" << new_velocity[0] << ", " << new_velocity[2] << ")\n";
    delete acceleration;
    return new_velocity;
};

//checked
float * NewVelocity(float target[], float self[])
{
    float * new_velocity = new float[2];
    float * distance = Distance(target, self);
    float velocity_mag = OriginalVelocityMagnitude();
    if (velocity_mag > maxium_velocity_magnitude)
        velocity_mag = maxium_velocity_magnitude;
    new_velocity[0] = velocity_mag * distance[0];
    new_velocity[1] = velocity_mag * distance[1];
    cout << "distance = (" << distance[0] << ", " << distance[2] << ")\n";
    cout << "new velocity = (" << new_velocity[0] << ", " << new_velocity[2] << ")\n";
    delete distance;
    return new_velocity;
};

//checked
float * NextPositionKinematic(float original_position[], float velocity[], float time_slot, int update)
{
    //p = p + v * t
    float * next_position = new float[2];
    next_position[0] = original_position[0] + velocity[0] * time_slot;
    next_position[1] = original_position[1] + velocity[1] * time_slot;
    cout << "In update " << update << " , the new position is: (" << next_position[0] << ", " << next_position[1] << "). \n";
    if (update == 1)
    {
        return next_position;
    }
    else 
    {
        return NextPositionKinematic(next_position, velocity, time_slot, update-1);
    }
    
};

float * NextPostitionSteering(float target[], float original_position[], float velocity[], float time_slot, int update)
{
    //p = p + v * t
    float * next_velocity = NewVelocitySteering(target, original_position, velocity, time_slot);
    float * next_position = new float[2];
    next_position[0] = original_position[0] + next_velocity[0] * time_slot;
    next_position[1] = original_position[1] + next_velocity[1] * time_slot;
    cout << "In update " << update << " , the new position is: (" << next_position[0] << ", " << next_position[1] << "). \n";
    if (update == 1)
    {
        delete next_velocity;
        return next_position;
    }
    else 
    {
        return NextPostitionSteering(target, next_position, next_velocity, time_slot, update-1);
    }
};

//checked
void KinematicSeek()
{
    float * velocity = NewVelocity(target_original_position, original_position);
    cout << "How many updates your want?\n";
    int update;
    cin >> update;
    float * position = NextPositionKinematic(original_position, velocity, time_slot, update);
    delete velocity;
    delete position;
};

//checked
void KinematicFlee()
{
    float * velocity = NewVelocity(original_position, target_original_position);
    cout << "How many updates your want?\n";
    int update;
    cin >> update;
    float * position = NextPositionKinematic(original_position, velocity, time_slot, update);
    delete velocity;
    delete position;
};


void SteeringSeek()
{
    cout << "How many updates your want?\n";
    int update;
    cin >> update;
    float * position = NextPostitionSteering(target_original_position, original_position, original_velocity, time_slot, update);
    delete position;
};
//End of question 1

//Question 2 starts


int main()
{
    int choose;
    cout << "1 for kinematic seek\n2 for kinematic flee\n3 for steering seek\n";
    cin >> choose;
    switch(choose)
    {
        case 1:
        KinematicSeek();
        break;
        case 2:
        KinematicFlee();
        break;
        case 3:
        SteeringSeek();
        break;
    }
    return 0;
}