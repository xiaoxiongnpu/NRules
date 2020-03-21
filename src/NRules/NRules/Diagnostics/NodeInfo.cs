using System.Collections.Generic;
using System.Linq;
using NRules.Rete;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Types of nodes in the rete network.
    /// </summary>
    public enum NodeType
    {
        Root,
        Type,
        Selection,
        AlphaMemory,
        Dummy,
        Join,
        Adapter,
        Exists,
        Aggregate,
        Not,
        Binding,
        BetaMemory,
        Terminal,
        Rule,
    }

    /// <summary>
    /// Node in the rete network graph.
    /// </summary>
#if (NET45 || NETSTANDARD2_0)
    [System.Serializable]
#endif
    public class NodeInfo
    {
        private static readonly string[] Empty = new string[0];

        internal static NodeInfo Create(RootNode node)
        {
            return new NodeInfo(NodeType.Root, string.Empty);
        }
        
        internal static NodeInfo Create(TypeNode node)
        {
            return new NodeInfo(NodeType.Type, node.FilterType.Name);
        }
        
        internal static NodeInfo Create(SelectionNode node)
        {
            var conditions = new[] {node.ExpressionElement.Expression.ToString()};
            return new NodeInfo(NodeType.Selection, string.Empty, conditions, Empty, Empty);
        }

        internal static NodeInfo Create(AlphaMemoryNode node, IAlphaMemory memory)
        {
            var items = memory.Facts.Select(f => f.Object.ToString());
            return new NodeInfo(NodeType.AlphaMemory, string.Empty, Empty, Empty, items);
        }

        internal static NodeInfo Create(JoinNode node)
        {
            var conditions = node.ExpressionElements.Select(c => c.Expression.ToString());
            return new NodeInfo(NodeType.Join, string.Empty, conditions, Empty, Empty);
        }

        internal static NodeInfo Create(NotNode node)
        {
            return new NodeInfo(NodeType.Not, string.Empty);
        }

        internal static NodeInfo Create(ExistsNode node)
        {
            return new NodeInfo(NodeType.Exists, string.Empty);
        }

        internal static NodeInfo Create(AggregateNode node)
        {
            var expressions = node.Expressions.Select(e => $"{e.Name}={e.Expression.ToString()}");
            return new NodeInfo(NodeType.Aggregate, node.Name, Empty, expressions, Empty);
        }

        internal static NodeInfo Create(ObjectInputAdapter node)
        {
            return new NodeInfo(NodeType.Adapter, string.Empty);
        }

        internal static NodeInfo Create(BindingNode node)
        {
            var expressions = new[] {node.ExpressionElement.Expression.ToString()};
            return new NodeInfo(NodeType.Binding, string.Empty, Empty, expressions, Empty);
        }

        internal static NodeInfo Create(BetaMemoryNode node, IBetaMemory memory)
        {
            var tuples = memory.Tuples.Select(
                t => string.Join(" || ", t.OrderedFacts().Select(f => f.Value).ToArray()));
            return new NodeInfo(NodeType.BetaMemory, string.Empty, Empty, Empty, tuples);
        }

        internal static NodeInfo Create(TerminalNode node)
        {
            return new NodeInfo(NodeType.Terminal, string.Empty);
        }

        internal static NodeInfo Create(RuleNode node)
        {
            return new NodeInfo(NodeType.Rule, node.CompiledRule.Definition.Name);
        }

        internal NodeInfo(NodeType nodeType, string details)
            : this(nodeType, details, Empty, Empty, Empty)
        {
        }

        internal NodeInfo(NodeType nodeType, string details, IEnumerable<string> conditions, IEnumerable<string> expressions, IEnumerable<string> items)
        {
            NodeType = nodeType;
            Details = details;
            Conditions = conditions.ToArray();
            Expressions = expressions.ToArray();
            Items = items.ToArray();
        }

        /// <summary>
        /// Type of the node in the rete network.
        /// </summary>
        public NodeType NodeType { get; }

        /// <summary>
        /// Additional node details.
        /// </summary>
        public string Details { get; }

        /// <summary>
        /// Match conditions.
        /// </summary>
        public string[] Conditions { get; }

        /// <summary>
        /// Additional node expressions.
        /// </summary>
        public string[] Expressions { get; }

        /// <summary>
        /// Facts/tuples currently associated with the node.
        /// </summary>
        public string[] Items { get; }
    }
}