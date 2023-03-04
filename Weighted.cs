/* Simulate unequal probability sampling using a fair die and a biased coin

The Alias Method presents an efficient means of randomly sampling discrete units with 
unequal probability. At its core, the method randomly selects a triangle and then flips
a biased coin to determine whether the triangle is sampled; with the probability of heads
being determined by the proportion of the total area occupied by the triangle. The arrays
created in the CreateAliasTables method provide a way to make this process more efficient.

*/
public class Weighted {

    public int Count;
    private Random rng;
    private double[] weights;
    private int[] alias;


    public Weighted(in double[] prob, int? seed){

        Count = prob.Length;


        if(seed is null){

            rng = new Random();

        } else {

            rng = new Random((int) seed);

        }
        

        alias = new int[Count];
        
        weights = new double[Count];
        
        Array.Copy(prob, weights, Count);



        SortedSet<AliasNode> bst = new SortedSet<AliasNode>(new AliasNodeComparer());

        for(int i = 0; i < Count; i++){

            bst.Add(new AliasNode(weights[i], i));

        }


        List<AliasNode> sortednodes = new List<AliasNode>();

        for(int i = 0; i < Count; i++){

            AliasNode? minnode = bst.Min;
            AliasNode? maxnode = bst.Max;

            if((minnode is null || (maxnode is null))){
                break;
            }

            bst.Remove(minnode);
            bst.Remove(maxnode);

            minnode.alias = maxnode.index;
            maxnode.prob = maxnode.prob - (1 - minnode.prob);

            sortednodes.Add(minnode);
            bst.Add(maxnode);

        }


        for(int i = 0; i < Count; i++){

            int index = sortednodes[i].index;
            prob[index] = sortednodes[i].prob;
            alias[index] = sortednodes[i].alias;

        }


    }



    /* Generate a sample of triangles */
    public List<int> Sample(int size){

        List<int> sample = new List<int>(size);


        // Using the "Alias Method" for generating an unequal probability sample.

        for(int i = 0; i < size; i++){

            int roll = rng.Next(0, Count);
            
            double flip = rng.NextDouble();

            if(flip <= weights[roll]){

                sample.Add(roll);

            } else {

                sample.Add(alias[roll]);

            }

        }

        return sample;

    }



    /* Binary Search Tree representation of the data */
    private class AliasNode {

        public double prob;
        public int alias;
        public int index;

        public AliasNode(double prob, int index){

            this.prob = prob;
            this.alias = index;
            this.index = index;

        }

    }


    /* Method for ordering AliasNodes in a Binary Search Tree */
    private class AliasNodeComparer : IComparer<AliasNode> {

        public int Compare(AliasNode? node1, AliasNode? node2){

            if ((node1 is null) || (node2 is null)){

                throw new ArgumentException("AliasNodes cannot be null!");

            }

            return (node1.prob).CompareTo(node2.prob);

        }

    }



    public string Print(int? digits){

        if(digits is null){
            digits = 4;
        }


        string buff = String.Format("Alias Table\n");

        for(int i = 0; i < Count; i++){

            buff += String.Format("i: {0}  Alias: {1}  Weight: {2}\n", i, alias[i], Math.Round(weights[i], (int) digits));

        }

        buff += "\n";

        return buff;

    }

}


