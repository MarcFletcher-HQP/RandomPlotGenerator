
#define DEBUG
#undef DEBUG

using System;
using System.Collections.Generic;

namespace RandomPlotGenerator;


/* Class: LocalPivotalMethod

It's all been building up to this! The Local Pivotal Method generates a
random sample that is Spatially Balanced (spread out). 

Regardless of how the candidate sample locations were derived, the local 
pivotal method will iteratively update the inclusion probability by making 
nodes square off against their neighbours in a coin flipping contest; the 
more a node wins, the more weighted in their favour the coin becomes. Nodes 
that lose all of their initial probability drop out of the contest. */
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



    /* Flip a weighted coin and update the selection probabilities. Each node bets
    its current chance of selection. Winner takes all, until full (i.e. it's selected).
    The current probability assigned to a point determines the weighting applied, points 
    containing more probability are given more weighting. 
        
    After each update, check whether a point can be selected, or excluded.
    */
    public void UpdateProbability(ref Point point1, ref Point point2){

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



    /* Generate a sample */
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

        KDTree tree = new KDTree(candidates, null);


        /* Typically the algorithm is close to convergence after visiting each node 
        at least once. Sometimes this can take a bit longer; how long is "a piece" 
        of string? 
        */

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

        }


        /* Sometimes not all of the points quite converged on one of the outcomes. If a point is more 
        likely than not to be included, then include it. */

        sample = candidates.FindAll((Point pt) => pt.prob > 0.5);


        return sample;

    }

}



