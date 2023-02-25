

#define DEBUG

using System;
using System.Collections.Generic;

namespace RandomPlotGenerator;


public class LocalPivotalMethod {

    private readonly Random rand;
    private double SelectionCutoff;


    public LocalPivotalMethod(int? seed, double? cutoff){

        if (seed is not null){
            rand = new Random((int) seed);
        } else {
            rand = new Random();
        }

        if(cutoff is null){
            SelectionCutoff = 0.95;
        } else {
            SelectionCutoff = (double) cutoff;
        }

    }



    public void UpdateProbability(ref Point point1, ref Point point2){

        /* if(point1.IsSelected() || point2.IsSelected()){
            
            return;

        } else if (point1.IsSelected()){

            point2.Exclude();
            return;

        } else if (point2.IsSelected()){

            point1.Exclude();
            return;

        } */




        /* Flip a weighted coin and, based on the outcome, update the selection probabilities.
        The current probability assigned to a point determines the weighting applied. 
        Points containing more probability are given more weighting. 
        
        After each update, check whether a point can be selected, or excluded.
        */

        double totalprob = point1.prob + point2.prob;

        if (totalprob < 1.0){

            double cutoff = point1.prob / totalprob;
            double weightedcoin = rand.NextDouble();

            point1.prob = totalprob * Convert.ToDouble(weightedcoin <= cutoff);     // Just imagine all the nanoseconds saved by eliminating one 'if' statement!
            point2.prob = totalprob - point1.prob;

        } else {

            double cutoff = (1 - point1.prob) / (2 - totalprob);
            double weightedcoin = rand.NextDouble();

            point1.prob = (totalprob - 1) * Convert.ToDouble(weightedcoin <= cutoff) + 1 * Convert.ToDouble(weightedcoin > cutoff);
            point2.prob = totalprob - point1.prob;

        }

        if(point1.prob >= SelectionCutoff){

            point1.Select();

        } else if (point1.prob <= (1 - SelectionCutoff)){

            point1.Exclude();

        }


        if(point2.prob >= SelectionCutoff){

            point2.Select();

        } else if (point2.prob <= (1 - SelectionCutoff)){

            point2.Exclude();

        }


        return;

    }




    public List<Point> SamplePoints(List<Point> candidates, int size){

        List<Point> sample = new List<Point>();


        if (size == 0){
            return sample;
        }

        if (candidates.Count <= size){

            sample.AddRange(candidates.GetRange(0, candidates.Count));
            return sample;

        }


        // Set initial probability for candidates

        double initprob = (double) size / (double) candidates.Count;

        foreach(Point pt in candidates){

            pt.SetInitialProb(initprob);

        }


        // Sort points into a KDTree

        KDTree tree = new KDTree();
        tree.Build(candidates, null);



        while(sample.Count < size){

            // Randomly pick a point, find it in the tree and find the nearest neighbour to that point.

            int index = rand.Next(0, candidates.Count - 1);

            Point point, neighbour;

            tree.Find(candidates[index], null, out point);

            if(point.IsExcluded() || point.IsSelected()){
                continue;
            }


            tree.SearchNN(point, true, out neighbour);



            // Update the selection probability for each point

            UpdateProbability(ref point, ref neighbour);



            // If a selection was made, then add it to the list

            if(point.IsSelected() && !sample.Contains(point)){

                sample.Add(point);

            }

            if(neighbour.IsSelected() && !sample.Contains(neighbour)){

                sample.Add(neighbour);

            }


            #if DEBUG

            Console.WriteLine(String.Format("Sample currently contains {0} points.", sample.Count));

            #endif

        }


        return sample;

    }



}



