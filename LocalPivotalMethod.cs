

#define DEBUG
#undef DEBUG

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


        /* Flip a weighted coin and, based on the outcome, update the selection probabilities.
        The current probability assigned to a point determines the weighting applied. 
        Points containing more probability are given more weighting. 
        
        After each update, check whether a point can be selected, or excluded.
        */

        double totalprob = point1.prob + point2.prob;

        if (totalprob < 1.0){

            double cutoff = point2.prob / totalprob;
            double weightedcoin = rand.NextDouble();

            point1.prob = totalprob * Convert.ToDouble(weightedcoin > cutoff);     // Just imagine all the nanoseconds saved by eliminating one 'if' statement!
            point2.prob = totalprob - point1.prob;

        } else {

            double cutoff = (1 - point2.prob) / (2 - totalprob);
            double weightedcoin = rand.NextDouble();

            point1.prob = 1 * Convert.ToDouble(weightedcoin <= cutoff) + (totalprob - 1) * Convert.ToDouble(weightedcoin > cutoff);
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




    public List<Point> SamplePoints(List<Point> candidates, int size, int? maxiter){

        List<Point> sample = new List<Point>();


        if (size == 0){
            return sample;
        }

        if (candidates.Count <= size){

            sample.AddRange(candidates.GetRange(0, candidates.Count));
            return sample;

        }


        if(maxiter is null){
            maxiter = candidates.Count * 10;
        }


        // Need to remove points that have been selected, or excluded, so that we don't end up in an infinite loop.

        List<int> sampleIdx = new List<int>();

        for( int i = 0; i < candidates.Count; i++ ){

            sampleIdx.Add(i);

        }


        // Set initial probability for candidates

        double initprob = (double) size / (double) candidates.Count;

        foreach(Point pt in candidates){

            pt.SetInitialProb(initprob);

        }


        // Sort points into a KDTree

        KDTree tree = new KDTree();
        tree.Build(candidates, null);



        for(int i = 0; i < maxiter; i++){

            // Randomly pick a point, find it in the tree and find the nearest neighbour to that point.

            if(sampleIdx.Count == 0){
                break;
            }


            int idx = rand.Next(0, sampleIdx.Count);        // Inclusive lower bound, exclusive upper bound [0,Count);

            int index = sampleIdx[idx];

            Point point = candidates[index]; 

            if(point.IsExcluded() || point.IsSelected()){
                sampleIdx.RemoveAt(idx);
                continue;
            }


            Point neighbour;

            tree.SearchNN(point, true, out neighbour);



            // Update the selection probability for each point

            UpdateProbability(ref point, ref neighbour);


            if (point.IsExcluded() || point.IsSelected()){

                sampleIdx.RemoveAt(idx);

            }



            #if DEBUG

            Console.WriteLine(String.Format("SamplePoints: Iteration: {0}  Remaining Points: {1}", i, sampleIdx.Count));

            #endif

        }


        sample = candidates.FindAll((Point pt) => pt.prob > 0.5);

        #if DEBUG

        for(int i = 0; i < sampleIdx.Count; i++){
            Point pt = candidates[sampleIdx[i]];
            Console.WriteLine(String.Format("SampleIdx[{0}]: {1}  ->  {2}  prob: {3}  status: {4}", i, sampleIdx[i], pt.Print(), pt.prob, pt.status));
        }

        #endif


        return sample;

    }




    private void PrintMultiPoint(in List<Point> sample){

        String buff = "MULTIPOINT(";

        for(int i = 0; i < sample.Count; i++){

            buff += ($"({sample[i].GetX()} {sample[i].GetY()})");

            if (i < sample.Count - 1){
                buff += ", ";
            }

        }

        buff += ")";

        Console.WriteLine(buff);

        return;

    }


}



