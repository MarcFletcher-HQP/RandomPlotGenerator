

using System;
using System.Collections.Generic;

namespace RandomPlotGenerator;


public class LocalPivotalMethod {

    private readonly Random rand;


    public LocalPivotalMethod(int? seed){

        if (seed is not null){
            rand = new Random((int) seed);
        } else {
            rand = new Random();
        }


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

            int index = rand.Next();

            Point point;

            tree.SearchNN(candidates[index], out point);

            



        }








    }



}