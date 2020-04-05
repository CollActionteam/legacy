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
      id
      description
      url
    }
    descriptiveImage {
      id
      description
      url
    }
    owner {
      fullName
      firstName
      lastName
    }
    tags {
      tag {
        name
      }
    }
    goal
    start
    end
    status
    target
    proposal
    remainingTime
    canSendProjectEmail
    displayPriority
    isActive
    isComingSoon
    isClosed
    isSuccessfull
    isFailed
    totalParticipants
    percentage
    numberProjectEmailsSent
  `,
};
