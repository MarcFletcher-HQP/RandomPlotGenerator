

using System;
using System.Collections.Generic;

namespace RandomPlotGenerator;


public class LocalPivotalMethod {


    public List<Point> SamplePoints(List<Point> candidates, int size){

        List<Point> sample = new List<Point>();

        if (size == 0){
            return sample;
        }


        // Sort points into a KDTree

        KDTree tree = new KDTree();
        tree.Build(candidates);


        double initprob = 1 / (double) size;

        List<double> prob = new List<double>(candidates.Count);

        for(int i = 0; i < prob.Count; i++){
            prob[i] = initprob;
        }


        // Randomly pick a point




    }



}