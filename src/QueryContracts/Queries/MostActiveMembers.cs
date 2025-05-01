using QueryContracts.Contract;

namespace QueryContracts.Queries;

public class MostActiveMembers
{
    public record Query(int Page, int PageSize) : IQuery.IQuery<Answer>;
    public record Answer(List<Member> Members);
    public record Member(string Name, int LastSixMonthsCount, int TotalCount, List<string> UpcomingEvents);
}