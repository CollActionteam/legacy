import gql from "graphql-tag";

export const Fragments = {
  category: gql`
    fragment Category on CategoryGraph {
      color
      colorHex
      id
      name
    }
  `,
  projectDetail: gql`
    fragment ProjectDetail on ProjectGraph {
      id
      name
      description
      url
      categoryId
      category {
        color
        colorHex
        name
      }
      descriptiveImage {
        filepath
        url
      }
      goal
      end
      target
      proposal
      remainingTime
      participantCounts {
        count
      }
      displayPriority
      isActive
      isComingSoon
      isClosed
    }
  `,
};
