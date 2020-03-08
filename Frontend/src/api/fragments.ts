export const Fragments = {
  projectDetail: `
    id
    name
    description
    url
    categories {
      category
    }
    bannerImage {
      filepath
      url
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
  `,
};
