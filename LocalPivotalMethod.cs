

#define DEBUG
//#undef DEBUG

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


/*         #if DEBUG

        Console.WriteLine(String.Format("UpdateProbability:  {0}: {1}  {2}: {3}", point1.Print(), point1.prob, point2.Print(), point2.prob));

        #endif */



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


/*         #if DEBUG

        Console.WriteLine(String.Format("UpdateProbability:  {0}: {1}  {2}: {3}", point1.Print(), point1.prob, point2.Print(), point2.prob));

        #endif */



        if(point1.prob == 1.0){

            point1.Select();

        } else if (point1.prob == 0.0){

            point1.Exclude();

        }


        if(point2.prob == 1.0){

            point2.Select();

        } else if (point2.prob == 0.0){

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



        // Index of selected points

/*         List<int> selectedIdx = new List<int>(candidates.Count);

        for(int i = 0; i < selectedIdx.Count; i++){
            selectedIdx[i] = 0;
        } */


/*         #if DEBUG
            int iter = 0;
        #endif */


        while(sample.Count < size){

            // Randomly pick a point, find it in the tree and find the nearest neighbour to that point.

            int index = rand.Next(0, candidates.Count - 1);

            Point point, neighbour;

            tree.Find(candidates[index], null, out point);

            tree.SearchNN(point, true, out neighbour);



            // Update the selection probability for each point

            UpdateProbability(ref point, ref neighbour);

/*             #if DEBUG

            Console.WriteLine(String.Format("SamplePoints:  {0}: {1}  {2}: {3}", point.Print(), point.prob, neighbour.Print(), neighbour.prob));

            #endif */



            // If a selection was made, then add it to the list

            /* selectedIdx[index] = point.IsSelected() ? 1 : 0; */

            if(point.IsSelected() && !sample.Contains(point)){

                sample.Add(point);

            } else if (point.IsExcluded() && sample.Contains(point)){

                sample.Remove(point);

            }


            if(neighbour.IsSelected() && !sample.Contains(neighbour)){

                sample.Add(neighbour);

            } else if (neighbour.IsExcluded() && sample.Contains(neighbour)){

                sample.Remove(neighbour);

            }


/*             #if DEBUG

            ++iter;

            if((iter % 5) == 0){

                Console.WriteLine(String.Format("Grid State at iteration {0}", iter));

                Console.WriteLine(String.Format("{0}", tree.Print()));


            }

            if((iter % 5) == 0){
                break;
            }

            #endif */

            #if DEBUG

            PrintMultiPoint(sample);

            /* for(int i = 0; i < sample.Count; i++ ){
                
                Console.WriteLine(String.Format("Sample {0}: {1}", i, sample[i].Print()));

            } */

            #endif

        }




        return sample;

    }



}



