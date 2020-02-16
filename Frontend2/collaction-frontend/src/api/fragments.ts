import gql from 'graphql-tag';

export const Fragments = {
  projectDetail: gql`
    fragment ProjectDetail on ProjectGraph {
      id
      name
      description
      url
      categories {
        category
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
      displayPriority
      isActive
      isComingSoon
      isClosed
      isSuccessfull
      isFailed
      totalParticipants
      percentage
    }
  `,
};
